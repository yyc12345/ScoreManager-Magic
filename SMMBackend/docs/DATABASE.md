# Database 文档

## 前置提醒

如果字段表述中提及“交由客户端解析”，则意味着此字段在服务器看来只作为普通的字符串来看待，客户端在接收到数据后再按照后文所述的结构进行解析

对于用户之类的，涉及安全类的代码，哈希使用SHA256，产生的数据用`VARCHAR(64)`进行存储。对于地图，哈希使用[xxHash](https://github.com/Cyan4973/xxHash)（实际使用库为[移植版本](https://www.nuget.org/packages/System.Data.HashFunction.xxHash)），产生的数据用`VARCHAR(16)`进行存储。数据库中的哈希值始终化为16进制小写形式存储

涉及时间存储的，统一以`BIGINT`存储，以UNIX时间戳进行解析

数据库中部分字段直接采用外置资源获取，如果后文中提及此字段是`EXTERNAL_RESOURCES`，那么则表明此段需要**交有客户端解析**，并且此字段的数据为JSON字典且具有如下标准格式：

|键名|值解析|
|:---|:---|
|origin|来源，用于区分此外置资源来自哪并在客户端代码中分别进行处理|
|url|资源地址|
|header|获取资源中需要用到的`HTTP Header`数据，或者是获取过程中需要用的参数数据。例如获取百度贴吧头像需要指定`HTTP Header`中的`Host`字段，可以写在这里|

数据库各个字段均不允许为`NULL`，后文谈及“为空”时均指为空串

## user表

### SQL代码

```sql
CREATE TABLE user (

# user basic info
sm_name TEXT NOT NULL,
sm_password VARCHAR(64) NOT NULL,
sm_salt INT UNSIGNED NOT NULL,
sm_token VARCHAR(32) NOT NULL,
sm_expireOn BIGINT UNSIGNED NOT NULL,
sm_permission TINYINT UNSIGNED NOT NULL,

# user additional info
sm_nickname TEXT NOT NULL,
sm_registration BIGINT UNSIGNED NOT NULL,
sm_avatar TEXT NOT NULL,
sm_playCount INT UNSIGNED NOT NULL,
sm_status VARCHAR(16) NOT NULL,

# database constraints
PRIMARY KEY(sm_name),
UNIQUE KEY(nickname)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

|字段名|解释|
|:---|:---|
|sm_name|用户名，主键。只允许英文，英文标点，数字和符号的组合|
|sm_password|用户密码，SHA256数值|
|sm_salt|用户最近一次登陆时所用盐。用户创建，登出时需要重置为一个随机数|
|sm_token|用户用于访问的token，未登录时为空。用户登出时需要置为空|
|sm_expireOn|用户token过期时间，为登录时刻加上一天的数值，UNIX时间戳。用户登出时需要置为0|
|sm_permission|用户权限，数值参考后文。|
|sm_nickname|用户昵称，唯一键|
|sm_registration|用户注册时间，UNIX时间戳|
|sm_avatar|用户头像，数据格式参考后文|
|sm_playCount|玩的次数，也与等级相关，算法于后文|
|sm_status|当前玩家正在玩的地图。在玩家登出后置为空|

#### sm_permission

表明用户的权限. 使用 `|` 运算符来组合各权限。

|权限数值（二进制）|权限名称|权限助记符|
|:---|:---|:---|
|0000|被Ban的用户（无权限）|none|
|0001|常规用户|user|
|0010|联赛直播推流员（有能力打开BTLD）|live|
|0100|联赛管理员（有能力管理比赛相关的表）|speedrun|
|1000|总管理（管理除比赛以外的表，例如用户之类的）|admin|

#### sm_avatar

用户头像，为`EXTERNAL_RESOURCES`，格式中某些字段解析如下：

`origin`：头像来源，目前支持`tieba`和`discord`。指定`tieba`时，`url`为百度i贴吧获取地址，此后分析html获得到真正的头像，`header`存储第二步获取所用的`Host`

#### sm_playCount

用户玩过的次数，每在submit中上传一次，这个数值就自增1。

此数值与用户等级相关，数值开二次方根并向下取整为用户等级。

## record表

### SQL代码

```sql
CREATE TABLE record (

# core data
sm_name TEXT NOT NULL,
sm_map VARCHAR(16) NOT NULL,
sm_score INT UNSIGNED NOT NULL,
sm_srTime INT UNSIGNED NOT NULL,
sm_referenceTime INT UNSIGNED NOT NULL,

# statistics field
sm_installedOn TINYINT UNSIGNED NOT NULL,
sm_lifeUp INT UNSIGNED NOT NULL,
sm_lifeLost INT UNSIGNED NOT NULL,
sm_extraPoint INT UNSIGNED NOT NULL,
sm_subExtraPoint INT UNSIGNED NOT NULL,
sm_trafo INT UNSIGNED NOT NULL,
sm_checkpoint INT UNSIGNED NOT NULL,
sm_mods INT UNSIGNED NOT NULL,

# verify field
sm_verify TINYINT UNSIGNED NOT NULL,
sm_localClock BIGINT UNSIGNED NOT NULL,
sm_timestamp BIGINT UNSIGNED NOT NULL,

# database constraints
FOREIGN KEY (sm_name) REFERENCES user(sm_name) ON DELETE CASCADE # sync delete
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

|字段名|解释|
|:---|:---|
|sm_name|上传用户名|
|sm_map|所打地图|
|sm_score|分数计分数（去除Level Bouns之类的数值，以初始数值999开始计算）|
|sm_srTime|由分数计计算的游戏时间|
|sm_referenceTime|用内置毫秒计时器计算的游戏时间|
|sm_installedOn|关卡被安装在哪一关|
|sm_lifeUp|吃到生命个数|
|sm_lifeLost|生命失去个数|
|sm_extraPoint|吃到分数球个数|
|sm_subExtraPoint|吃到分数的小球的个数|
|sm_trafo|通过变球器个数|
|sm_checkpoint|通过盘点个数|
|sm_mods|此次记录附加的mod，数据格式在下文|
|sm_verify|bsm输出Token验证是否通过。|
|sm_localClock|客户端发起请求时的当地时间的时间戳|
|sm_timestamp|插入此行时的UNIX时间戳，此字段由服务器负责判定与写入|

#### sm_mods

表明进行此次记录时附加的mod. 使用 `|` 运算符来组合各权限。

|Mod数值（二进制）|权限名称|权限助记符|
|:---|:---|:---|
|0|无Mod|none|
|1|HS（Highscore，HS玩法，道具不全吃，就不上传成绩）|hs|

## map表

### SQL代码

```sql
CREATE TABLE map (

# core data
sm_hash VARCHAR(16) NOT NULL,
sm_name TEXT NOT NULL,
sm_i18n TEXT NOT NULL,
sm_status TINYINT UNSIGNED NOT NULL,
sm_download TEXT NOT NULL,
sm_preview TEXT NOT NULL,
sm_wiki TEXT NOT NULL,

# database constraints
PRIMARY KEY(sm_hash)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

|字段名|解释|
|:---|:---|
|sm_hash|地图哈希|
|sm_name|地图源语言名称|
|sm_i18n|地图名的国际化语言列表，数据结构见后文|
|sm_status|地图状态，数据结构见后文|
|sm_download|地图下载地址|
|sm_preview|地图预览图下载地址|
|sm_wiki|指向描述此地图的wiki的页面|

#### sm_i18n

地图名的国际化语言列表，为JSON字典，键为[.Net Framework中CultureInfo.Name](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.name?view=netframework-4.5)合法的值，值为翻译文本。

#### sm_status

|数值|状态名称|助记符|
|:---|:---|:---|
|0|Rank的地图（即可以参与排行榜计算的）|ranked|
|1|此地图是某个地图的旧版本，或者开发版本，不加入排行榜。|upgradeable|
|2|此地图不值一提，或者存在设计上的问题，例如不能通关，需要依赖当前SMM不支持的Mod等，不能加入排行榜。|graveyard|

#### sm_download

地图下载地址，为`EXTERNAL_RESOURCES`，格式中某些字段解析如下：

`origin`：目前支持`ballancewiki`，之后的`url`就是下载地址

#### sm_preview

地图下载地址，为`EXTERNAL_RESOURCES`，格式中某些字段解析如下：

`origin`：目前支持`ballancewiki`，之后的`url`就是下载地址

## leaderboard表

### SQL代码

```sql
CREATE TABLE leaderboard (

# core data
sm_name TEXT NOT NULL,
sm_map VARCHAR(16) NOT NULL,
sm_time INT UNSIGNED NOT NULL,
sm_pp INT UNSIGNED NOT NULL,
sm_video TEXT NOT NULL,
sm_timestamp BIGINT UNSIGNED NOT NULL,
sm_mods INT UNSIGNED NOT NULL,

# statistics field
sm_installedOn TINYINT UNSIGNED NOT NULL,
sm_lifeUp INT UNSIGNED NOT NULL,
sm_lifeLost INT UNSIGNED NOT NULL,
sm_extraPoint INT UNSIGNED NOT NULL,
sm_subExtraPoint INT UNSIGNED NOT NULL,
sm_trafo INT UNSIGNED NOT NULL,
sm_checkpoint INT UNSIGNED NOT NULL,

# database constraints
FOREIGN KEY (sm_name) REFERENCES user(sm_name) ON DELETE CASCADE # sync delete
FOREIGN KEY (sm_map) REFERENCES map(sm_hash) ON DELETE CASCADE # sync delete
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

#### sm_id

自增的比赛ID


## competition表

### SQL代码

```sql
CREATE TABLE competition (
sm_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
sm_result TEXT,
sm_startDate BIGINT,
sm_endDate BIGINT,
sm_judgeEndDate BIGINT,
sm_map VARCHAR(64),
sm_banMap TEXT,
sm_cdk TEXT,
sm_winner TEXT,

PRIMARY KEY ( sm_id )
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

#### sm_id

自增的比赛ID

#### sm_result

比赛结果，JSON列表，其中每个项为下述对象：

|字段|类型|解释|
|:---|:---|:---|
|name|string|用户名|
|result|int|成绩，-1表示作弊，-2表示未参赛|
|link|string|记录视频地址|

#### sm_banMap

参与者的Ban图，JSON列表，每一项是地图的hash值

#### sm_cdk

比赛的CDK

## participant表

### SQL代码

```sql
CREATE TABLE participant (
sm_id BIGINT UNSIGNED,
sm_participant TEXT
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

#### sm_id

该参赛者参加的比赛的ID

#### sm_participant

参赛者用户名

## registry表

### SQL代码

```sql
CREATE TABLE registry (
sm_user TEXT,
sm_tournament TEXT
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

#### sm_user

注册用户

#### sm_tournament

注册的联赛

## tournament表

### SQL代码

```sql
CREATE TABLE tournament (
sm_tournament TEXT,
sm_startDate BIGINT,
sm_endDate BIGINT,
sm_schedule LONGTEXT
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

#### sm_tournament

联赛名称

#### sm_startDate

注册开始时间

#### sm_endDate

注册结束时间

#### sm_schedule

以JSON格式存储的赛程表

# Database文档

## user表

### SQL代码

```sql
CREATE TABLE user (
sm_name TEXT,
sm_password VARCHAR(64),
sm_registration BIGINT UNSIGNED,
sm_priority TINYINT UNSIGNED,
sm_salt INT,
sm_token VARCHAR(32),
sm_expireOn BIGINT UNSIGNED
);
```

### 字段解释

#### sm_name

用户名

#### sm_priority

用户权限


表明用户的权限. 使用 `|` 运算符来组合各权限。

|Priority value (BIN)|Priority name|Priority code (written in code)|
|:---|:---|:---|
|0000 (No permission)|Banned user|none|
|0001|Normal user|user|
|0010|System admin|admin|

## record表

### SQL代码

```sql
CREATE TABLE record (
sm_name TEXT,

sm_installedOn TINYINT,
sm_map VARCHAR(64),

sm_score INT,
sm_srTime INT,

sm_lifeUp INT,
sm_lifeLost INT,
sm_extraPoint INT,
sm_subExtraPoint INT,
sm_trafo INT,
sm_checkpoint INT,
sm_verify TINYINT UNSIGNED,
sm_token TEXT,

sm_localUTC BIGINT UNSIGNED,
sm_serverUTC BIGINT UNSIGNED
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

暂无

## map表

### SQL代码

```sql
CREATE TABLE map (
sm_name TEXT,
sm_i8n TEXT,
sm_hash VARCHAR(64),
sm_tournament TEXT
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

#### sm_name

地图的名称，使用地图的源语言名称.

#### sm_i18n

地图的英文名称

#### sm_hash

此地图的hash值，使用SHA256算法，转换为小写的16进制字符串输出。

#### sm_tournament

本地图已被加入哪个联赛的图池中

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

比赛结果，JSON列表，其中每一项遵守下述格式：

|助记符|解释|
|:---|:---|
|name|用户名|
|result|成绩，-1表示作弊，-2表示未参赛|

#### sm_banMap

参与者的Ban图

#### sm_cdk

比赛的CDK

## competitionParticipant表

### SQL代码

```sql
CREATE TABLE competitionParticipant (
sm_id BIGINT UNSIGNED,
sm_participant TEXT
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 字段解释

#### sm_id

该参赛者参加的比赛的ID

#### sm_participant

参赛者用户名

## participant表

### SQL代码

```sql
CREATE TABLE participant (
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

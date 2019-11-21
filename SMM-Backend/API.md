# API 文档

## 前言

### 通用返回

|字段名|数据类型|描述|
|:---:|:---:|:---:|
|code|int|响应码，指示操作是否成功|
|err|string|错误信息，如果没有错误，则为空字符串|
|data|-|返回的数据，如果没有返回的数据，则此值为空字符串|

后文所有返回部分写的数据为`data`部分的数据

若后文返回写为无返回，则表明程序通常是要去读取`code`字段判断是否执行成功

### 数据传输格式

* 后文中的bool类型数据传输将通过int类型进行实现

## version.php

获取服务器版本

### 请求

无请求主体

### 返回

|字段名|数据类型|描述|
|:---:|:---:|:---:|
|ver|string|服务器当前版本字符串|

## salt.php

获取登录所用盐

### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|name|string|必选参数|用户名|

### 返回

|字段名|数据类型|描述|
|:---:|:---:|:---:|
|rnd|string|返回用于下一步登录的盐|

## login.php

登陆账号

### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|name|string|必选参数|用户名|
|hash|string|必选参数|带盐经过计算的用于登录的字符串|

### 返回

|字段名|数据类型|描述|
|:---:|:---:|:---:|
|token|string|用于后续所有操作的token|
|priority|int|当前账户拥有的权限|

## logout.php

账号下线

### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|需要下线的token|

### 返回

无返回

## submit.php

提交用户成绩记录

### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|用户的token|
|installOn|int|必选参数|关卡安装关|
|map|string|必选参数|地图hash|
|score|int|必选参数|分数|
|srTime|int|必选参数|毫秒计时|
|lifeUp|int|必选参数|获得生命|
|lifeLost|int|必选参数|失去生命|
|extraPoint|int|必选参数|分数球个数|
|subExtraPoint|int|必选参数|分数球内小球个数|
|trafo|int|必选参数|过变球器个数|
|checkpoint|int|必选参数|过盘点个数|
|verify|bool|必选参数|验证是否通过|
|bsmToken|int|必选参数|本次bsm验证token|
|localTime|int|必选参数|本地UTC时间|

### 返回

无返回

<!-- server only-->

## operationUser.php

操纵用户相关数据，管理员接口

### 查询接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`query`|
|filterRules|string (JSON字典)|必选参数，字典可以为空|用于筛选结果，为字典，可用字段：`name`（string，名称筛选），`vagueName`（bool，启用模糊名称）|
|neededReturn|string (JSON列表)|必选参数，列表不得为空|指定需要返回的字段，每一项为`string`且名称与数据库内匹配|

#### 返回

返回`neededReturn`指定的字段的列表

### 添加接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`add`|
|newValues|string (JSON字典)|必选参数，字典必须包含所有可用字段|用于添加新项的初始数据，为字典，可用字段：`name`（string，名称），`password`（string，密码），`priority`（int，权限）|

#### 返回

无返回

### 删除接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`delete`|
|target|string (JSON列表)|必选参数，列表不得为空|用于确认作用对象，列表每一项是作用对象的`sm_name`字段值|

#### 返回

无返回

### 更新接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`update`|
|target|string (JSON列表)|必选参数，列表不得为空|用于确认作用对象，列表每一项是作用对象的`sm_name`字段值|
|newValues|string (JSON字典)|必选参数，字典不得为空|用于更新数值，为字典，可用字段：`password`（string，密码），`priority`（int，权限），`expireOn`（int，token过期时间）|

#### 返回

无返回

## operationCompetition.php

操纵比赛相关数据，管理员接口

### 查询接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`query`|
|filterRules|string (JSON字典)|必选参数，字典可以为空|用于筛选结果，为字典，可用字段：`id`（int列表，id筛选，筛选任意符合列表中的id的项），`name`（string，名称筛选），`startDate`（long，起始时间筛选），`endDate`（long，结束时间筛选），`judgeDate`（long，判定结束时间筛选），`cdk`（string，cdk筛选），`map`（string，地图筛选）|
|neededReturn|string (JSON列表)|必选参数，列表不得为空|指定需要返回的字段，每一项为`string`且名称与数据库内匹配|

#### 返回

返回`neededReturn`指定的字段的列表

### 添加接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`add`|
|newValues|string (JSON字典)|必选参数，字典必须包含所有可用字段|用于添加新项的初始数据，为字典，可用字段：`startDate`（string，开始时间），`endDate`（string，结束时间），`judgeEndDate`（string，判断结束时间）|

#### 返回

无返回

### 删除接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`delete`|
|target|string (JSON列表)|必选参数，列表不得为空|用于确认作用对象，列表每一项是作用对象的`sm_id`字段值|

#### 返回

无返回

### 更新接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`update`|
|target|string|必选参数，列表不得为空|用于确认作用对象，是作用对象的`sm_id`字段值|
|newValues|string (JSON字典)|必选参数，字典不得为空|用于更新数值，为字典，可用字段：`result`（string（JSON字典），比赛结果纪录），`map`（string，比赛地图），`banMap`（string（JSON列表），比赛被Ban的地图），`winner`（string，比赛胜者）|

#### 返回

无返回

## operationRecord.php

操纵纪录相关数据，管理员接口

### 查询接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`query`|
|filterRules|string (JSON字典)|必选参数，字典可以为空|用于筛选结果，为字典，可用字段：`installedOn`（int，安装关卡筛选），`name`（string，名称筛选），`startDate`（long，起始时间筛选），`endDate`（long，结束时间筛选），`score`（int，分数筛选），`time`（int，时间筛选），`map`（string，地图筛选）|
|neededReturn|string (JSON列表)|必选参数，列表不得为空|指定需要返回的字段，每一项为`string`且名称与数据库内匹配|

#### 返回

返回`neededReturn`指定的字段的列表

## operationTournament.php

操纵联赛相关数据，管理员接口

### 查询接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`query`|
|filterRules|string (JSON字典)|必选参数，字典可以为空|用于筛选结果，为字典，可用字段：`name`（string，联赛名称）|
|neededReturn|string (JSON列表)|必选参数，列表不得为空|指定需要返回的字段，每一项为`string`且名称与数据库内匹配|

#### 返回

返回`neededReturn`指定的字段的列表

### 添加接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`add`|
|newValues|string (JSON字典)|必选参数，字典必须包含所有可用字段|用于添加新项的初始数据，为字典，可用字段：`startDate`（string，注册开始时间），`endDate`（string，注册结束时间），`name`（string，联赛名称）|

#### 返回

无返回

### 删除接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`delete`|
|target|string (JSON列表)|必选参数，列表不得为空|用于确认作用对象，列表每一项是作用对象的`sm_tournament`字段值|

#### 返回

无返回

### 更新接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`update`|
|target|string|必选参数，列表不得为空|用于确认作用对象，是作用对象的`sm_id`字段值|
|newValues|string (JSON字典)|必选参数，字典不得为空|用于更新数值，为字典，可用字段：`startDate`（string，注册开始时间），`endDate`（string，注册结束时间），`schedule`（string（超长JSON数据），比赛赛程完整安排）|

#### 返回

无返回


## operationRegistry.php

操纵联赛注册相关数据，管理员接口

### 查询接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`query`|
|filterRules|string (JSON字典)|必选参数，字典可以为空|用于筛选结果，为字典，可用字段：`user`（string，用户筛选），`vagueName`（bool，启用模糊用户名称），`tournament`（string，联赛名称筛选）|
|neededReturn|string (JSON列表)|必选参数，列表不得为空|指定需要返回的字段，每一项为`string`且名称与数据库内匹配|

#### 返回

返回`neededReturn`指定的字段的列表

### 添加接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`add`|
|newValues|string (JSON字典)|必选参数，字典必须包含所有可用字段|用于添加新项的初始数据，为字典，可用字段：`user`（string，用户），`tournament`（string，所属联赛名称）|

#### 返回

无返回

### 删除接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`delete`|
|target|string (JSON字典)|必选参数，字典必须包含所有可用字段|用于确认作用对象，，为字典，可用字段：`user`（string，用户），`tournament`（string，所属联赛名称）|

#### 返回

无返回


## operationMap.php

操纵地图相关数据，管理员接口

### 查询接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`query`|
|filterRules|string (JSON字典)|必选参数，字典可以为空|用于筛选结果，为字典，可用字段：`name`（string，原名称筛选），`i18n`（string，英文名称筛选），`hash`（string，hash筛选），`tournament`（string，联赛筛选）|
|neededReturn|string (JSON列表)|必选参数，列表不得为空|指定需要返回的字段，每一项为`string`且名称与数据库内匹配|

#### 返回

返回`neededReturn`指定的字段的列表

### 添加接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`add`|
|newValues|string (JSON字典)|必选参数，字典必须包含所有可用字段|用于添加新项的初始数据，为字典，可用字段：`name`（string，原名称），`i18n`（string，英文名称），`hash`（string，hash），`tournament`（string，所属联赛）|

#### 返回

无返回

### 删除接口

#### 请求

|字段名|数据类型|数据条件|描述|
|:---:|:---:|:---:|:---:|
|token|string|必选参数|管路员的token，用于确认权限|
|method|string|必选参数|固定值，为`delete`|
|target|string (JSON列表)|必选参数，列表不得为空|用于确认作用对象，列表每一项是作用对象的`sm_hash`字段值|

#### 返回

无返回



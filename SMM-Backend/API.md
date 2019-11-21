# API 文档
<!-- normal user-->

## 前言

### 通用返回

|Field|Description|
|:---|:---|
|code|A response code, indicate this operation's result|
|err|Error discription|
|data|Returned data. If response don't have any return value, this property keep blank string|

All of following php interfaces' response data is `data`'s data structure.

若后文返回写为无返回，则`data`区块为空字符串，且通常程序是要去判断`code`字段判断是否执行成功

### 数据传输格式

* 后文中的bool类型数据传输将通过int类型进行实现

## version.php

Get services' version

### Priority requirement

None

### Request

None

### Response

|Field|Description|
|:---|:---|
|ver|A string indicating current server version.|

## salt.php

Pre-step for login.

### Priority requirement

None

### Request

Request type: POST

|Field|Description|
|:---|:---|
|name|User name|

### Response

|Field|Description|
|:---|:---|
|rnd|A string. For following steps|

## login.php

Login your account.

### Priority requirement

None

### Request

Request type: POST

|Field|Description|
|:---|:---|
|name|User name|
|hash|A computed string|

### Response

|Field|Description|
|:---|:---|
|token|A string for following access|
|priority|A int indicating current user's permission|

## logout.php

Logout your account.

### Priority requirement

None

### Request

Request type: POST

|Field|Description|
|:---|:---|
|token|The token provided in login process|

### Response

No data.

## submit.php

Submit player score.

### Priority requirement

`user`

### Request

Request type: POST

|Field|Description|
|:---|:---|
|token|The token provided in login process|
|installOn|Correspond with database field|
|map|Correspond with database field|
|score|Correspond with database field|
|srTime|Correspond with database field|
|lifeUp|Correspond with database field|
|lifeLost|Correspond with database field|
|extraPoint|Correspond with database field|
|subExtraPoint|Correspond with database field|
|trafo|Correspond with database field|
|checkpoint|Correspond with database field|
|verify|Correspond with database field|
|bsmToken|Correspond with database field|
|localTime|Correspond with database field|

### Response

No data.


<!-- server only-->

## init.php

Init SMM server.

### Priority requirement

Priority is not suit for this request.

### Request

Request type: POST

|Field|Description|
|:---|:---|
|su|The string defined in adminacc.php|

### Response

No data.

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



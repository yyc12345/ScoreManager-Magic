# API document
<!-- normal user-->

## Universal Response

|Field|Description|
|:---|:---|
|code|A response code, indicate this operation's result|
|err|Error discription|
|data|Returned data. If response don't have any return value, this property keep blank string|

All of following php interfaces' response data is `data`'s data structure.

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

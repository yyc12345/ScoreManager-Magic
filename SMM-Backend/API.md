# API document
<!-- normal user-->

## Universal Response

|Field|Description|
|:---|:---|
|code|A response code, indicate this operation's result|
|error|Error discription|
|data|Returned data. Can be NULL|

All of following php interfaces' response data is `data`'s data structure.

## salt.php

Pre-step for login.

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

## logout.php

Logout your account.

### Request

Request type: POST

|Field|Description|
|:---|:---|
|token|The token provided in login process|

### Response

No data.

## submit.php

Submit player score.

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

## getTournament.php

## getCompetition.php

## getMapHash.php

<!-- tournament operation-->

## getUserScore.php

<!-- admin operation-->

## user.php

## tournament.php

## competition.php

## mapHash.php

<!-- server only-->

## init.php

Init SMM server.

### Request

Request type: POST

|Field|Description|
|:---|:---|
|su|The string defined in adminacc.php|

### Response

No data.

## config.php

Should not be visited via normal request.

## utilities.php

Should not be visited via normal request.

## preconfig.php

Should not be visited via normal request.

## database.php

Should not be visited via normal request.
# API document
<!-- normal user-->

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

HTTP 200 for normal logout.

## submit.php

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

## config.php

Should not be visited via normal request.

## utilities.php

Should not be visited via normal request.
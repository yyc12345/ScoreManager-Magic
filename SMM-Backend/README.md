# SMM-Backend

This is the backend for ScoreManager and BTLD. In the following contents, I will describe each modules' function.

This backend uses PHP. You should configure correct PHP environment and set up Nginx or other Internet server well.

This backend needs HTTPS protocol to ensure each data is safe.

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

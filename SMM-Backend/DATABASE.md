# Database document

## user Table

### SQL code

```sql
CREATE TABLE user (
sm_name TEXT,
sm_password CHAR(64),
sm_registration BIGINT,
sm_priority TINYINT,
sm_salt INT,
sm_token CHAR(32),
sm_expireOn BIGINT
);
```

### Fields description

#### sm_name

The name of this user. Should not contain `,` or `#`.

#### sm_priority

The power of this user. Following chart will describe each priority and upper right inherit lower right.

|Priority value|Priority name|
|:---|:---|
|0|Banned user|
|1|Normal user|
|2|Tournament live watcher|
|3|Tournament live admin|
|4|Record authenticator|
|5|System admin|

## record Table

### SQL code

```sql
CREATE TABLE record (
sm_name TEXT,

sm_installedOn TINYINT,
sm_hash CHAR(64),

sm_score INT,
sm_srTime INT,
sm_counter TINYTEXT,

sm_lifeUp INT,
sm_lifeLost INT,
sm_extraPoint INT,
sm_subExtraPoint INT,
sm_trafo INT,
sm_checkpoint INT,
sm_verify TINYINT,

sm_localTime BIGINT,
sm_localUTC BIGINT,
sm_serverUTC BIGINT
);
```

### Fields description

## map Table

### SQL code

```sql
CREATE TABLE map (
sm_name TEXT,
sm_author TEXT,
sm_hash CHAR(64)
);
```

### Fields description

#### sm_name

The name of this name. Use original name.

#### sm_author

The author for this map. Use original name.

#### sm_hash

The hash for this map file. Use SHA256. Convert to HEX string with low case.

## tournament Table

### SQL code

```sql
CREATE TABLE tournament (
sm_tournament TEXT,
sm_mapPool TEXT,
sm_regStartDate BIGINT,
sm_regEndDate BIGINT,
sm_rootCompetition CHAR(32)
);
```

### Fields description

#### sm_tournament

The name of this tournament. Should be unique.

#### sm_mapPool

The map pool for this tournament. A list. Item is the hash of each map and seperator is `,`.

#### sm_regStartDate

The start date of registration.

#### sm_regEndDate

The end date of registration.

#### sm_rootCompetition

A "competition quote". This is the final competition in this tournament. Following dependency tree, a full competition tree of thie tournament will be build. This value is served for map construction.

## competition Table

### SQL code

```sql
CREATE TABLE competition (
sm_id CHAR(32),
sm_type TINYINT,
sm_parents TEXT,
sm_arrange TEXT,
sm_winner TEXT
);
```

### Fields description

#### sm_id

A unique string. Generated from GUID.

#### sm_type

The type of this competition. Following chart provide detailed message.

|Value|Meaning|
|:---|:---|
|0|Single cycle competition|
|1|Knockout competition|

#### sm_parents

A "competition quote" list. Indicate this competition's participant. Spectator is `,`.

#### sm_arrange

A hash list. Each item is sm_id in arrangement table. Spectator is `,`.

#### sm_winner

The winner name of this competition.


## arrangement Table

### SQL code

```sql
CREATE TABLE arrangement (
sm_id CHAR(32),
sm_red TEXT,
sm_redRes TEXT,
sm_blue TEXT,
sm_blueRes TEXT,
sm_startDate BIGINT,
sm_endDae BIGINT,
sm_evalStartDate BIGINT,
sm_evalEndDate BIGINT,
sm_map CHAR(64),
sm_tournament TEXT,
sm_winner TEXT
);
```

### Fields description

#### sm_id

A unique string. Generated from GUID.

#### sm_red / sm_blue

The participant for this tournament.

## Attached message

What is "competition quote"?

A "competition quote" following this syntax:

`(u|s|k)#id`

`u` mean user, `s` mean single cycle competition and `k` mean knockout competition.

If you use `u`, the id is user name. If you use `s` or `k`, the id is competition id.
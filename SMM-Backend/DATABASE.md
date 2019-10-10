# Database document

## user Table

### SQL code

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

### Fields description

#### sm_name

The name of this user. Should not contain `,` or `#`.

#### sm_priority

The power of this user. Following chart will describe each priority and upper right inherit lower right.

|Priority value|Priority name|
|:---|:---|
|0|Banned user|
|1|Normal user|
|2|Tournament live admin|
|3|Record authenticator|
|4|System admin|

## record Table

### SQL code

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

### Fields description

## map Table

### SQL code

```sql
CREATE TABLE map (
sm_name TEXT,
sm_author TEXT,
sm_hash VARCHAR(64)
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
sm_tournament TEXT
);
```

### Fields description

#### sm_tournament

The name of this tournament. Should be unique.

## participant Table

### SQL code

```sql
CREATE TABLE participant (
sm_id TEXT,
sm_type TINYINT UNSIGNED,
sm_registration BIGINT UNSIGNED,
sm_tournament TEXT
);
```

### Fields description

#### sm_id

If this item is user, it is user name. Otherwise it is map hash.

#### sm_type

The type of this participant. Following chart provides detailed message.

|Value|Meaning|
|:---|:---|
|0|The user which take part in this tournament|
|1|The map which will be used in this tournament|

#### sm_registration

The date when this item is added into this tournament.

#### sm_tournament

Which tournament this item want to take part in.

## competition Table

### SQL code

```sql
CREATE TABLE competition (
sm_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
sm_red TEXT,
sm_redRes BIGINT,
sm_blue TEXT,
sm_blueRes BIGINT,
sm_startDate BIGINT,
sm_endDate BIGINT,
sm_map VARCHAR(64),
sm_tournament TEXT,
sm_winner TEXT
);
```

### Fields description

#### sm_id

A unique string. Generated from GUID.

#### sm_red / sm_blue

The participant for this tournament.

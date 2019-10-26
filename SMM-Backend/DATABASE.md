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

The power of this user. Use `|` operator to construct permission.

|Priority value (BIN)|Priority name|Priority code (written in code)|
|:---|:---|:---|
|0000 (No permission)|Banned user|none|
|0001|Normal user|user|
|0010|Tournament live admin|live|
|0100|Record authenticator|speedrun|
|1000|System admin|admin|

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

<?php

require_once "config.php";
require_once "utilities.php";
require_once "adminacc.php";

//check parameter
if (!array_key_exists("su", $_POST)) {
    SetHTTPCode(400);
    die();
}
if ($_POST["su"] != $INIT_ROOT_ACCOUNT["su"]) {
    SetHTTPCode(403);
    die();
}

//establish database
try {
    $conn = new PDO("mysql:host=" . $GLOBAL_CONFIG["database"]["url"] . ":" . $GLOBAL_CONFIG["database"]["port"] . ";dbname=" . $GLOBAL_CONFIG["database"]["db"], $GLOBAL_CONFIG["database"]["user"], $GLOBAL_CONFIG["database"]["password"]);
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $conn->exec("set names utf8");

    //set up table
    $conn->exec("CREATE TABLE user (
sm_name TEXT,
sm_password VARCHAR(64),
sm_registration BIGINT,
sm_priority TINYINT,
sm_salt INT,
sm_token VARCHAR(32),
sm_expireOn BIGINT
)");
    $conn->exec("CREATE TABLE record (
sm_name TEXT,

sm_installedOn TINYINT,
sm_hash VARCHAR(64),

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
sm_token TEXT,

sm_localTime BIGINT,
sm_localUTC BIGINT,
sm_serverUTC BIGINT
)");
    $conn->exec("CREATE TABLE map (
sm_name TEXT,
sm_author TEXT,
sm_hash VARCHAR(64)
)");
    $conn->exec("CREATE TABLE tournament (
sm_tournament TEXT
);");
    $conn->exec("CREATE TABLE participant (
sm_id TEXT,
sm_type TINYINT,
sm_registration BIGINT,
sm_tournament TEXT
)");
    $conn->exec("CREATE TABLE competition (
sm_id VARCHAR(32),
sm_red TEXT,
sm_redRes INT,
sm_blue TEXT,
sm_blueRes INT,
sm_startDate BIGINT,
sm_endDae BIGINT,
sm_map VARCHAR(64),
sm_tournament TEXT,
sm_winner TEXT
)");

    $user_hash = hash("sha256", $INIT_ROOT_ACCOUNT["password"]);
    $regDate = time();
    $expireDate = $regDate + 60 * 60 * 24;
    $stmt = $conn->prepare("INSERT INTO user (sm_name, sm_password, sm_registration, sm_priority, sm_salt, sm_token, sm_expireOn)
VALUES (?, ?, ?, 4, 0, '', ?)");
    $stmt->bindParam(1, $INIT_ROOT_ACCOUNT["user"], PDO::PARAM_STR);
    $stmt->bindParam(2, $user_hash, PDO::PARAM_STR);
    $stmt->bindParam(3, $regDate, PDO::PARAM_INT);
    $stmt->bindParam(4, $expireDate, PDO::PARAM_INT);
    $stmt->execute();
    $conn = null;

} catch (Exception $e) {
    echo $e->getMessage();
    SetHTTPCode(500);
    die();
}

SetHTTPCode(200);
echo 0;

?>
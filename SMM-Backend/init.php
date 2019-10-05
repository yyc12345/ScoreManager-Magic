<?php

require_once "config.php";
require_once "utilities.php";

//check parameter
if (!array_key_exists("su", $_POST)) {
    SetHTTPCode(400);
    die();
}
if ($_POST["su"] != $GLOBAL_CONFIG["su"]) {
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
sm_password CHAR(64),
sm_registration BIGINT,
sm_priority TINYINT,
sm_salt INT,
sm_token CHAR(32),
sm_expireOn BIGINT
)");
    $conn->exec("CREATE TABLE record (
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
)");
    $conn->exec("CREATE TABLE map (
sm_name TEXT,
sm_author TEXT,
sm_hash CHAR(64)
)");
    $conn->exec("CREATE TABLE tournament (
sm_tournament TEXT,
sm_participant TEXT,
sm_maps TEXT
)");
    $conn->exec("CREATE TABLE competition (
sm_id CHAR(32),
sm_red TEXT,
sm_blue TEXT,
sm_startDate BIGINT,
sm_endDae BIGINT,
sm_map CHAR(64),
sm_tournament TEXT
)");

$conn = null;

} catch (Exception $e) {
    echo $e->getMessage();
    SetHTTPCode(500);
    die();
}

SetHTTPCode(200);
echo 0;

?>
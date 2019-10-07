<?php

require_once "database.php";
require_once "utilities.php";

if (!CheckParameter($_POST, array("name"))){
    echo json_encode(GetUniversalReturn(400, "Invalid parameter"));
    die(); 
}

try {
    $db = new database();
    $db->lockdb();
    if(!$db->checkUser($_POST["name"])) {
        echo json_encode(GetUniversalReturn(400, "Invalid user name"));
        die(); 
    }

    $salt = $db->generateSalt($_POST["name"]);
    $db->unlockdb();
    $db = NULL;

    $retData = GetUniversalReturn(200, "OK");
    $retData["data"] = array("salt" => $salt);
    echo json_encode($retData);

} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(500, $e->getMessage()));
    die();
}

?>
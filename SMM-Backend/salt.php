<?php

require_once "database.php";
require_once "utilities.php";

if (!CheckParameter($_POST, array("name"))){
    echo json_encode(GetUniversalReturn(false, "Invalid parameter"));
    die(); 
}

try {
    $db = new database();
    $db->lockdb();
    if(!$db->checkUser($_POST["name"])) {
        echo json_encode(GetUniversalReturn(false, "Invalid user name"));
        die(); 
    }

    $salt = $db->generateSalt($_POST["name"]);
    $db->unlockdb();
    $db = NULL;

    $retData = GetUniversalReturn();
    $retData["data"] = array("salt" => $salt);
    echo json_encode($retData);

} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(false, $e->getMessage()));
    die();
}

?>
<?php

require_once "database.php";
require_once "utilities.php";

if (!CheckParameter($_POST, array("name", "hash"))){
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

    $token = $db->authUser($_POST["name"], $_POST["hash"]);
    $db->unlockdb();
    $db = NULL;
    if ($token == "") {
        echo json_encode(GetUniversalReturn(400, "Fail to auth login"));
    } else {
        $retData = GetUniversalReturn(200, "OK");
        $retData["data"] = array("token" => $token);
        echo json_encode($retData);
    }
    
} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(500, $e->getMessage()));
    die();
}

?>
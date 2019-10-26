<?php

require_once "database.php";
require_once "utilities.php";

if (!CheckParameter($_POST, array("name", "hash"))){
    echo json_encode(GetUniversalReturn(false , "Invalid parameter"));
    die(); 
}

try {
    $db = new database();
    $db->lockdb();
    if(!$db->checkUser($_POST["name"])) {
        echo json_encode(GetUniversalReturn(false, "Invalid user name"));
        die(); 
    }

    $token = $db->authUser($_POST["name"], $_POST["hash"]);
    $db->unlockdb();
    $db = NULL;
    if ($token == "") {
        echo json_encode(GetUniversalReturn(false, "Fail to auth login")); 
    } else {
        //get priority again
        $priority = $db->getPriority($token);

        echo json_encode(GetUniversalReturn(true, "OK", array("token" => $token,
                                                            "priority" => $priority)));
    }
    
} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(false, $e->getMessage()));
    die();
}

?>
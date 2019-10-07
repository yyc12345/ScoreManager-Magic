<?php

require_once "database.php";
require_once "utilities.php";

if (!CheckParameter($_POST, array("token"))){
    echo json_encode(GetUniversalReturn(400, "Invalid parameter"));
    die(); 
}

try {
    $db = new database();
    $db->lockdb();
    if(!$db->checkLogin($_POST["token"])) {
        echo json_encode(GetUniversalReturn(400, "Invalid token"));
        die(); 
    }

    $db->resetToken($_POST["token"]);
    $db->unlockdb();
    $db = NULL;
    echo json_encode(GetUniversalReturn());
    
} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(500, $e->getMessage()));
    die();
}

?>
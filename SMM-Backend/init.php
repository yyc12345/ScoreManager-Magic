<?php

require_once "database.php";
require_once "utilities.php";
require_once "preconfig.php";

//check parameter
if (!CheckParameter($_POST, array("su"))) {
    echo json_encode(GetUniversalReturn(false, "Invalid parameter"));
    die(); 
}
if ($_POST["su"] != $INIT_ROOT_ACCOUNT["su"]) {
    echo json_encode(GetUniversalReturn(false , "Fail to auth parameter"));
    die();
}

//establish database
try {
    $db = new database();
    $db->init();
    
    //add init user
    $user_hash = hash("sha256", $INIT_ROOT_ACCOUNT["password"]);
    $db->lockdb();
    $db->addUser($INIT_ROOT_ACCOUNT["user"], $user_hash, 4);
    $db->unlockdb();

    $db = NULL;
    echo json_encode(GetUniversalReturn());
    
} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(false, $e->getMessage()));
    die();
}

?>
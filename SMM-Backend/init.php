<?php

require_once "database.php";

$INIT_ROOT_ACCOUNT = array(
    "user" => "root",
    "password" => "password"
);

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
    echo "Done";
    
} catch (Exception $e) {
    echo "$e->getMessage()";
    die();
}

?>
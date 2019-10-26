<?php

require_once "database.php";
require_once "utilities.php";

if (!CheckParameter($_POST, array("token", "installOn", "map", "score", "srTime", "lifeUp", "lifeLost", "extraPoint", "subExtraPoint", "trafo", "checkpoint", "verify", "bsmToken", "localTime"))){
    echo json_encode(GetUniversalReturn(false, "Invalid parameter"));
    die(); 
}

try {
    $db = new database();
    $db->lockdb();
    if(!$db->checkToken($_POST["token"])) {
        echo json_encode(GetUniversalReturn(false, "Invalid token"));
        die(); 
    }
    //check permission
    if(!(CheckPriority($db->getPriority($token), "user"))) {
        echo json_encode(GetUniversalReturn(false, "No permission"));
        die(); 
    }

    $db->addSubmit($_POST["token"], $_POST["installOn"], $_POST["map"], $_POST["score"], $_POST["srTime"], $_POST["lifeUp"], $_POST["lifeLost"], $_POST["extraPoint"], $_POST["subExtraPoint"], $_POST["trafo"], $_POST["checkpoint"], $_POST["verify"], $_POST["bsmToken"], $_POST["localTime"]);
    $db->unlockdb();
    $db = NULL;

    echo json_encode(GetUniversalReturn());
    
} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(false, $e->getMessage()));
    die();
}

?>
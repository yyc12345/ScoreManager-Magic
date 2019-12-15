<?php

require_once "database.php";
require_once "utilities.php";
require_once "databasehelper.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("token", "newPassword"))) throw new Exception("Invalid parameter");

    $db = new database();
    $db->lockdb();
    if(!$db->checkToken($_POST["token"])) throw new Exception("Invalid token");
    if(!(\SMMUtilities\CheckPriority($db->getPriority($_POST["token"]), \SMMDataStructure\EnumUserPriority::user))) throw new Exception("No permission");

    //submit
    $stmt = $db->conn->prepare("UPDATE user SET sm_password = ? WHERE sm_token = ?");
    $stmt->bindParam(1 ,$_POST["newPassword"] , PDO::PARAM_STR);
    $stmt->bindParam(2 ,$_POST["token"], PDO::PARAM_STR);
    $stmt->execute();
    
    $db->unlockdb();
    $db = NULL;

    echo json_encode(\SMMUtilities\GetUniversalReturn());
    
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
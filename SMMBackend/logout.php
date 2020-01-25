<?php

require_once "database.php";
require_once "utilities.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("token"))) throw new Exception("Invalid parameter");

    $db = new database();
    $db->lockdb();
    if(!$db->checkToken($_POST["token"])) throw new Exception("Invalid token");

    //$db->resetToken($_POST["token"]);
    $preventAttackRnd = \SMMUtilities\GetRandomNumber();
    $stmt = $db->conn->prepare("UPDATE user SET sm_token = '', sm_expireOn = 0, sm_salt = ? WHERE sm_token = ?");
    $stmt->bindParam(1, $preventAttackRnd, PDO::PARAM_INT); //set salt to random number to prevent try login
    $stmt->bindParam(2, $_POST["token"], PDO::PARAM_STR);
    $stmt->execute();

    $db->unlockdb();
    $db = NULL;
    echo json_encode(\SMMUtilities\GetUniversalReturn());
    
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
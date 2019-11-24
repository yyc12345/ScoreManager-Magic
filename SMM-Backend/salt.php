<?php

require_once "database.php";
require_once "utilities.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("name"))) throw new Exception("Invalid parameter");
    
    $db = new database();
    $db->lockdb();
    if(!$db->checkUser($_POST["name"])) throw new Exception("Invalid user name");
    
    //start stmt to do job
    $salt = \SMMUtilities\GetRandomNumber();
    $stmt = $db->$conn->prepare("UPDATE user SET sm_salt = ? WHERE sm_name = ?");
    $stmt->bindParam(1, $salt, PDO::PARAM_INT);
    $stmt->bindParam(2, $user, PDO::PARAM_STR);
    $stmt->execute();

    $db->unlockdb();
    $db = NULL;

    echo json_encode(GetUniversalReturn(true, "OK", array("rnd" => $salt)));

} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(false, $e->getMessage()));
}

?>
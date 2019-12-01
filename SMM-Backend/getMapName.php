<?php

require_once "database.php";
require_once "utilities.php";
require_once "datastructure.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("token", "mapHash"))) throw new Exception("Invalid parameter");

    $db = new database();
    $db->lockdb();
    if(!$db->checkToken($_POST["token"])) throw new Exception("Invalid token");
    if(!(\SMMUtilities\CheckPriority($db->getPriority($_POST["token"]), \SMMDataStructure\EnumUserPriority::user))) throw new Exception("No permission");

    //get answer
    $stmt = $db->conn->prepare("SELECT sm_name, sm_i18n FROM map WHERE sm_hash = ?");
    $stmt->bindParam(1 ,$_POST["mapHash"] , PDO::PARAM_STR);
    $stmt->execute();

    //detect exist
    $data = $stmt->fetchAll(PDO::FETCH_ASSOC);
    if (count($data) == 0) throw new Exception("No matched map hash");

    $db->unlockdb();
    $db = NULL;

    echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", array("name"=>$data[0]["sm_name"], "i18n"=>$data[0]["sm_i18n"])));
    
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
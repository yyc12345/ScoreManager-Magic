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

    //decode map hash param
    $mapList = json_decode($_POST["mapHash"], true);
    $length = count($mapList);
    $constrcutHelper = array();
    foreach ($mapList as $i) 
        $constrcutHelper[] = " sm_hash = ? ";
    $mapQueryStatement = join(" || ", $constrcutHelper);

    //get answer
    $stmt = $db->conn->prepare("SELECT sm_name, sm_i18n, sm_hash FROM map WHERE (" . $mapQueryStatement . ")");
    for($i=0; $i<$length; $i++) 
        $stmt->bindParam($i+1 ,$mapList[$i] , PDO::PARAM_STR);
    $stmt->execute();

    //get result
    $data = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $db->unlockdb();
    $db = NULL;

    echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", $data));
    
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
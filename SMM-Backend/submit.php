<?php

require_once "database.php";
require_once "utilities.php";
require_once "datastructure.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("token", "installOn", "map", "score", "srTime", "lifeUp", "lifeLost", "extraPoint", "subExtraPoint", "trafo", "checkpoint", "verify", "bsmToken", "localTime")))
        throw new Exception("Invalid parameter");

    $db = new database();
    $db->lockdb();
    if(!$db->checkToken($_POST["token"])) throw new Exception("Invalid token");
    if(!(CheckPriority($db->getPriority($_POST["token"]), \SMMDataStructure\EnumUserPriority::user))) throw new Exception("No permission");

    //get server time
    $serverTime = time();
    //query name
    $user=$db->getUserFromToken($_POST["token"]);

    //submit
    $stmt = $db->conn->prepare("INSERT INTO record (sm_name, sm_installOn, sm_map, sm_score, sm_srTime, sm_lifeUp, sm_lifeLost, sm_extraPoint, sm_subExtraPoint, sm_trafo, sm_checkpoint, sm_verify, sm_token, sm_localUTC, sm_serverUTC) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
    $stmt->bindParam(1 ,$user , PDO::PARAM_STR);
    $stmt->bindParam(2 ,$_POST["installOn"] , PDO::PARAM_INT);
    $stmt->bindParam(3 ,$_POST["map"] , PDO::PARAM_STR);
    $stmt->bindParam(4 ,$_POST["score"] , PDO::PARAM_INT);
    $stmt->bindParam(5 ,$_POST["srTime"] , PDO::PARAM_INT);
    $stmt->bindParam(6 ,$_POST["lifeUp"] , PDO::PARAM_INT);
    $stmt->bindParam(7 ,$_POST["lifeLost"] , PDO::PARAM_INT);
    $stmt->bindParam(8 ,$_POST["extraPoint"] , PDO::PARAM_INT);
    $stmt->bindParam(9 ,$_POST["subExtraPoint"] , PDO::PARAM_INT);
    $stmt->bindParam(10 ,$_POST["trafo"] , PDO::PARAM_INT);
    $stmt->bindParam(11 ,$_POST["checkpoint"] , PDO::PARAM_INT);
    $stmt->bindParam(12 ,$_POST["verify"] , PDO::PARAM_INT);
    $stmt->bindParam(13 ,$_POST["bsmToken"] , PDO::PARAM_STR);
    $stmt->bindParam(14 ,$_POST["localTime"] , PDO::PARAM_INT);
    $stmt->bindParam(15 ,$serverTime , PDO::PARAM_INT);
    $stmt->execute();
    
    $db->unlockdb();
    $db = NULL;

    echo json_encode(GetUniversalReturn());
    
} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(false, $e->getMessage()));
}

?>
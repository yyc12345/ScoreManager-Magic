<?php

require_once "database.php";
require_once "utilities.php";
require_once "datastructure.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("token"))) throw new Exception("Invalid parameter");

    $db = new database();
    $db->lockdb();
    if(!$db->checkToken($_POST["token"])) throw new Exception("Invalid token");
    if(!(CheckPriority($db->getPriority($_POST["token"]), \SMMDataStructure\EnumUserPriority::user))) throw new Exception("No permission");

    //submit
    $srvTime = time();
    //query all matched tournament (start date before tournament end 21 days or end date after tournament end 3 day)
    $stmt = $db->conn->prepare("SELECT sm_tournament, sm_startDate, sm_endDate FROM tournament WHERE (sm_startDate > ? && sm_startDate < ?) || (sm_endDate > ? && sm_endDate < ?)");
    $stmt->bindParam(1, $srvTime, PDO::PARAM_INT);
    $stmt->bindParam(2, $srvTime + 60 * 60 * 24 * 21, PDO::PARAM_INT);
    $stmt->bindParam(3, $srvTime - 60 * 60 * 24 * 3, PDO::PARAM_INT);
    $stmt->bindParam(4, $srvTime, PDO::PARAM_INT);
    $stmt->execute();
    
    $allTournament = $stmt->fetchAll(PDO::FETCH_ASSOC);
    $length = count($allTournament);

    //filter tournament and get whether participant
    $allTourName = array();
    foreach($allTournament as $i) 
        $allTourName[] = "sm_tournament = ?";
    //construct query statement
    $matchedTourStatement = join(" || ", $allTourName);

    //query user
    $user = $db->getUserFromToken($_POST["token"]);
    //query participant
    $stmt2 = $db->conn->prepare("SELECT sm_tournament WHERE (" . $matchedTourStatement . ") && sm_user = ?");
    for($i=0; $i<$length; $i++)
        $stmt2->bindParam($i+1, $allTourName[$i], PDO::PARAM_STR);
    $stmt2->bindParam($length, $user, PDO::PARAM_STR);
    $stmt2->execute();

    $participanted = $stmt2->fetchAll(PDO::FETCH_COLUMN, 0);
    foreach($allTournament as $value) 
        $value["participated"] = (int)in_array($value["sm_tournament"], $participanted);
    
    $db->unlockdb();
    $db = NULL;

    echo json_encode(GetUniversalReturn(true, "OK", $allTournament));
    
} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(false, $e->getMessage()));
}

?>
<?php

require_once "database.php";
require_once "utilities.php";
require_once "datastructure.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("token"))) throw new Exception("Invalid parameter");

    $db = new database();
    $db->lockdb();
    if(!$db->checkToken($_POST["token"])) throw new Exception("Invalid token");
    if(!(\SMMUtilities\CheckPriority($db->getPriority($_POST["token"]), \SMMDataStructure\EnumUserPriority::user))) throw new Exception("No permission");

    //submit
    $srvTime = time();
    $srvStart = \SMMUtilities\DateAddDays($srvTime, 21);
    $srvEnd = \SMMUtilities\DateAddDays($srvTime, -3);
    //query all matched tournament (start date before tournament end 21 days or end date after tournament end 3 day)
    $stmt = $db->conn->prepare("SELECT sm_tournament, sm_startDate, sm_endDate FROM tournament WHERE (sm_startDate > ? && sm_startDate < ?) || (sm_endDate > ? && sm_endDate < ?) || (sm_startDate < ? && sm_endDate > ?)");
    $stmt->bindParam(1, $srvTime, PDO::PARAM_INT);
    $stmt->bindParam(2, $srvStart, PDO::PARAM_INT);
    $stmt->bindParam(3, $srvEnd, PDO::PARAM_INT);
    $stmt->bindParam(4, $srvTime, PDO::PARAM_INT);
    $stmt->bindParam(5, $srvTime, PDO::PARAM_INT);
    $stmt->bindParam(6, $srvTime, PDO::PARAM_INT);
    $stmt->execute();
    
    $allTournament = $stmt->fetchAll(PDO::FETCH_ASSOC);
    $length = count($allTournament);
    if ($length == 0) goto end; //if don't have any tournament, goto end directly.

    //filter tournament and get whether participant
    $allTourName = array();
    $allTourNameStruct = array();
    foreach($allTournament as $i) {
        $allTourNameStruct[] = "sm_tournament = ?";
        $allTourName[] = $i["sm_tournament"];
    }
    //construct query statement
    $matchedTourStatement = join(" || ", $allTourNameStruct);

    //query user
    $user = $db->getUserFromToken($_POST["token"]);
    //query participant
    $stmt2 = $db->conn->prepare("SELECT sm_tournament FROM participant WHERE (" . $matchedTourStatement . ") && sm_user = ?");
    for($i=0; $i<$length; $i++)
        $stmt2->bindParam($i+1, $allTourName[$i], PDO::PARAM_STR);
    $stmt2->bindParam($length + 1, $user, PDO::PARAM_STR);
    $stmt2->execute();

    $participanted = $stmt2->fetchAll(PDO::FETCH_COLUMN, 0);
    for($i = 0; $i < $length; $i++) 
        $allTournament[$i]["participated"] = in_array($allTournament[$i]["sm_tournament"], $participanted);
    
end:
    $db->unlockdb();
    $db = NULL;

    echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", $allTournament));
    
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
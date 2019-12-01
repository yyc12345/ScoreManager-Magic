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

    //get current time
    $srvTime = time();
    $srvStart = \SMMUtilities\DateAddDays($srvTime, 5);
    $srvEnd = \SMMUtilities\DateAddDays($srvTime, -1);
    //query all matched competition (start date before competiton end 5 days or end date after competiton end 1 day)
    $stmt = $db->conn->prepare("SELECT sm_id, sm_startDate, sm_endDate, sm_map, sm_cdk WHERE (sm_startDate > ? && sm_startDate < ?) || (sm_endDate > ? && sm_endDate < ?) || (sm_startDate < ? && sm_endDate > ?)");
    $stmt->bindParam(1, $srvTime, PDO::PARAM_INT);
    $stmt->bindParam(2, $srvStart, PDO::PARAM_INT);
    $stmt->bindParam(3, $srvEnd, PDO::PARAM_INT);
    $stmt->bindParam(4, $srvTime, PDO::PARAM_INT);
    $stmt->bindParam(5, $srvTime, PDO::PARAM_INT);
    $stmt->bindParam(6, $srvTime, PDO::PARAM_INT);
    $stmt->execute();

    //=======================================================get all competition
    $allCompetition = $stmt->fetchAll(PDO::FETCH_ASSOC);
    if(count($allCompetition) == 0) goto end; //if competition is empty, goto end directly.

    //get all id and try query user competition
    $allId = array();
    foreach($allCompetition as $i) 
        $allId[] = "sm_id = " . $i["sm_id"];
    //construct query statement
    $matchedCompetitionStatement = join(" || ", $allId);
    //query user
    $user = $db->getUserFromToken($_POST["token"]);

    //=======================================================query competition participant
    $stmt2 = $db->conn->prepare("SELECT sm_id WHERE (" . $matchedCompetitionStatement . ") && sm_participant = ?");
    $stmt2->bindParam(1, $user, PDO::PARAM_STR);
    $stmt2->execute();
    //get the first column data
    $allParticipantedComp = $stmt2->fetchAll(PDO::FETCH_COLUMN, 0);
    //filter all competition and removed useless item
    foreach($allCompetition as $i) {
        if (!in_array($i["sm_id"], $allParticipantedComp, TRUE))
            unset($allCompetition[array_search($i, $allCompetition)]);
    }

    //=======================================================query related participant
    $allId = array();
    foreach($allCompetition as $i) 
        $allId[] = "sm_id = " . $i["sm_id"];
    //construct query statement
    $matchedCompetitionStatement = join(" || ", $allId);

    $stmt3 = $db->conn->prepare("SELECT sm_id, sm_participant WHERE (" . $matchedCompetitionStatement . ")");
    $stmt3->execute();
    //collect data
    $participant = array();
    foreach($stmt3->fetchAll(PDO::FETCH_GROUP) as $key=>$value) {
        $cache = array();
        foreach($value as $i) $cache[] = $i["sm_participant"];
        $participant[$key] = $cache;
    }
    //bind to final query data
    foreach($allCompetition as $i)
        $i["sm_participant"] = $participant[$i["sm_id"]];
    
    //=======================================================hide cdk
    foreach($allCompetition as $i) {
        if ($i["sm_startDate"] > $srvTime)
            $i["cdk"] = "";
    }

end:
    $db->unlockdb();
    $db = NULL;
    //use array_values to clean key and output list
    echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", array_values($allCompetition)));
    
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
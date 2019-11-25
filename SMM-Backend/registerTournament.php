<?php

require_once "database.php";
require_once "utilities.php";
require_once "datastructure.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("token", "tournament"))) throw new Exception("Invalid parameter");

    $db = new database();
    $db->lockdb();
    if(!$db->checkToken($_POST["token"])) throw new Exception("Invalid token");
    if(!(CheckPriority($db->getPriority($_POST["token"]), \SMMDataStructure\EnumUserPriority::user))) throw new Exception("No permission");

    //check tournament
    $stmt = $db->$conn->prepare("SELECT sm_tournament FROM tournament WHERE sm_tournament = ?");
    $stmt->bindParam(1 ,$_POST["tournament"] , PDO::PARAM_STR);
    $stmt->execute();

    if (count($stmt->fetchAll(PDO::FETCH_ASSOC)) == 0) throw new Exception("No matched tournament");

    //check whether registered
    $user = $db->getUserFromToken($_POST["token"]);
    $stmt2 = $db->$conn->prepare("SELECT * FROM participant WHERE sm_tournament = ? && sm_user = ?");
    $stmt2->bindParam(1 ,$_POST["tournament"] , PDO::PARAM_STR);
    $stmt2->bindParam(2 ,$user , PDO::PARAM_STR);
    $stmt2->execute();

    if (count($stmt2->fetchAll(PDO::FETCH_ASSOC)) != 0) throw new Exception("You have registered previously");

    //register
    $stmt3 = $db->$conn->prepare("INSERT INTO participant (sm_user, sm_tournament) VALUES (?, ?)");
    $stmt3->bindParam(1, $_POST["tournament"], PDO::PARAM_STR);
    $stmt3->bindParam(2, $user, PDO::PARAM_STR);
    $stmt3->execute();

    $db->unlockdb();
    $db = NULL;

    echo json_encode(GetUniversalReturn());
    
} catch (Exception $e) {
    echo json_encode(GetUniversalReturn(false, $e->getMessage()));
}

?>
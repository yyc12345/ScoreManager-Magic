<?php

require_once "database.php";
require_once "utilities.php";
require_once "datastructure.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("name"))) throw new Exception("Invalid parameter");

    if (\SMMUtilitie\CheckHardcodeParam($_POST, array("method" => "query"))) {
        //query, check param
        //$filterRules = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["filterRules"]);
        //$neededReturn = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["neededReturn"]);
        //if (!CheckOptionalParam($neededReturn, SMMDataStructure\DatabaseField\GetTableUser(), 1)) throw new Exception("Invalid parameter");
        //if (!CheckOptionalParam($filterRules, array("name"), 0)) throw new Exception("Invalid parameter");

    } else if (\SMMUtilitie\CheckHardcodeParam($_POST, array("method" => "add"))) {
        //add, check param


        
    } else if (\SMMUtilitie\CheckHardcodeParam($_POST, array("method" => "delete"))) {
        //rm, check param


        
    } else if (\SMMUtilitie\CheckHardcodeParam($_POST, array("method" => "update"))) {
        //update, check param


        
    } else throw new Exception("Invalid parameter");
    /*
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("name"))) throw new Exception("Invalid parameter");
    
    $db = new database();
    $db->lockdb();
    if(!$db->checkUser($_POST["name"])) throw new Exception("Invalid user name");
    
    //start stmt to do job
    $salt = \SMMUtilities\GetRandomNumber();
    $stmt = $db->conn->prepare("UPDATE user SET sm_salt = ? WHERE sm_name = ?");
    $stmt->bindParam(1, $salt, PDO::PARAM_INT);
    $stmt->bindParam(2, $_POST["name"], PDO::PARAM_STR);
    $stmt->execute();

    $db->unlockdb();
    $db = NULL;

    echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", array("rnd" => $salt)));
    */
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
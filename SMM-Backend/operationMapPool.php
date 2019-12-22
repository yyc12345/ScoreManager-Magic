<?php

require_once "database.php";
require_once "utilities.php";
require_once "databasehelper.php";

try {
    $db = new database();
    $db->lockdb();

    //confirm token and permission
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("token"))) throw new Exception("Invalid parameter");
    if(!$db->checkToken($_POST["token"])) throw new Exception("Invalid token");
    if(!(\SMMUtilities\CheckPriority($db->getPriority($_POST["token"]), \SMMDataStructure\EnumUserPriority::admin))) throw new Exception("No permission");

    if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "query"))) {
        //query, check param
        if (!\SMMUtilities\CheckNecessityParam($_POST, array("filterRules"))) throw new Exception("Invalid parameter");
        //decode filter rules and needed return
        $decodeFilter = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["filterRules"]);

        //contruct statement
        $whereStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateFilterStatement($decodeFilter, array(
            "hash" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_STR, "sm_hash"),
            "tournament" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_STR, "sm_tournament")
        ),array(), $whereStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('SELECT * FROM mapPool' . ( $whereStatement == "" ? "" : " WHERE " . $whereStatement));
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", $stmt->fetchAll(PDO::FETCH_ASSOC)));

    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "add"))) {
        //add, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("newValues"))) throw new Exception("Invalid parameter");
        $decodeNewValues = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["newValues"]);
        if(!\SMMUtilities\CheckNecessityParam($decodeNewValues, array("hash", "tournament"))) throw new Exception("Invalid parameter");

        //construct statement
        $insertKeyStatement = "";
        $insertValueStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateSeparatedStatement($decodeNewValues, 
        array("hash" => new \SMMDatabaseStatement\ParamValueUserInput(PDO::PARAM_STR, "sm_hash"),
            "tournament" => new \SMMDatabaseStatement\ParamValueUserInput(PDO::PARAM_STR, "sm_tournament")),
        array(), $insertKeyStatement, $insertValueStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('INSERT mapPool ' . $insertKeyStatement . ' VALUES ' . $insertValueStatement);
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());

    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "delete"))) {
        //rm, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("target"))) throw new Exception("Invalid parameter");
        $decodeTarget = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["target"]);
        if(!\SMMUtilities\CheckNecessityParam($decodeTarget, array("hash", "tournament"))) throw new Exception("Target is not limited");

        //invoke stmt directly
        $stmt = $db->conn->prepare('DELETE FROM mapPool WHERE (sm_hash = ? && sm_tournament = ?)');
        $stmt->bindParam(1, $decodeTarget["hash"], PDO::PARAM_STR);
        $stmt->bindParam(2, $decodeTarget["tournament"], PDO::PARAM_STR);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());

    } else throw new Exception("Invalid parameter");
    
    $db->unlockdb();
    $db = NULL;
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
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
    if(!(\SMMUtilities\CheckPriority($db->getPriority($_POST["token"]), \SMMDataStructure\EnumUserPriority::speedrun))) throw new Exception("No permission");

    if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "query"))) {
        //query, check param
        if (!\SMMUtilities\CheckNecessityParam($_POST, array("filterRules"))) throw new Exception("Invalid parameter");
        //decode filter rules and needed return
        $decodeFilter = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["filterRules"]);

        //contruct statement
        $whereStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateFilterStatement($decodeFilter, array(
            "name" => new \SMMDatabaseStatement\ParamFilterUserInput('LIKE', PDO::PARAM_STR, "sm_name")
        ),array(), $whereStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('SELECT * FROM tournament' . ( $whereStatement == "" ? "" : " WHERE " . $whereStatement));
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", $stmt->fetchAll(PDO::FETCH_ASSOC)));

    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "add"))) {
        //add, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("newValues"))) throw new Exception("Invalid parameter");
        $decodeNewValues = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["newValues"]);
        if(!\SMMUtilities\CheckNecessityParam($decodeNewValues, array("name", "startDate", "endDate"))) throw new Exception("Invalid parameter");

        //construct statement
        $insertKeyStatement = "";
        $insertValueStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateSeparatedStatement($decodeNewValues, 
        array("name" => new \SMMDatabaseStatement\ParamSeperatedUserInput(PDO::PARAM_STR, "sm_tournament"),
            "startDate" => new \SMMDatabaseStatement\ParamSeperatedUserInput(PDO::PARAM_INT, "sm_startDate"),
            "endDate" => new \SMMDatabaseStatement\ParamSeperatedUserInput(PDO::PARAM_INT, "sm_endDate")),
        array("sm_schedule" => new \SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_STR, "[]")),
        $insertKeyStatement, $insertValueStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('INSERT tournament ' . $insertKeyStatement . ' VALUES ' . $insertValueStatement);
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());
        
    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "delete"))) {
        //rm, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("target"))) throw new Exception("Invalid parameter");

        //bind param and execute
        $stmt = $db->conn->prepare('DELETE FROM tournament WHERE sm_tournament = ?');
        $stmt->bindParam(1, $_POST["target"], PDO::PARAM_STR);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());
        
    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "update"))) {
        //update, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("target", "newValues"))) throw new Exception("Invalid parameter");
        $decodeNewValues = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["newValues"]);
        if(!\SMMUtilities\CheckOptionalParam($decodeNewValues, array("schedule", "startDate", "endDate"), 1)) throw new Exception("Invalid parameter");

        //construct statement
        $setStatement = "";
        $whereStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateFilterStatement($decodeNewValues,
        array("startDate" => new \SMMDatabaseStatement\ParamFilterUserInput("=", PDO::PARAM_INT, "sm_startDate"),
            "endDate" => new \SMMDatabaseStatement\ParamFilterUserInput("=", PDO::PARAM_INT, "sm_endDate"),
            "schedule" => new \SMMDatabaseStatement\ParamFilterUserInput("=", PDO::PARAM_STR, "sm_schedule")),
        array(), $setStatement, $args);
        \SMMDatabaseStatement\GenerateFilterStatement(array(), array(),
        array("sm_tournament" => new \SMMDatabaseStatement\ParamFilterConstantInput("=", PDO::PARAM_STR, $_POST["target"])),
        $whereStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('UPDATE participant SET ' . $setStatement . ' WHERE ' . $whereStatement);
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());
        
    } else throw new Exception("Invalid parameter");
    
    $db->unlockdb();
    $db = NULL;
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
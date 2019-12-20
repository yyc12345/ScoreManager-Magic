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
            "name" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_STR, "sm_name"),
            "installedOn" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_INT, "sm_installedOn"),
            "startDate" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_INT, "sm_startDate"),
            "endDate" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_INT, "sm_endDate"),
            "score" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_INT, "sm_score"),
            "time" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_INT, "sm_srTime"),
            "map" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_STR, "sm_map")
        ),array(), $whereStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('SELECT * FROM record' . ( $whereStatement == "" ? "" : " WHERE " . $whereStatement));
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", $stmt->fetchAll(PDO::FETCH_ASSOC)));

    } else throw new Exception("Invalid parameter");

    $db->unlockdb();
    $db = NULL;
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
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
            "name" => new \SMMDatabaseStatement\ParamFilterUserInput('LIKE', PDO::PARAM_STR, "sm_name")
        ),array(), $whereStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('SELECT * FROM user' . ( $whereStatement == "" ? "" : " WHERE " . $whereStatement));
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", $stmt->fetchAll(PDO::FETCH_ASSOC)));

    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "add"))) {
        //add, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("newValues"))) throw new Exception("Invalid parameter");
        $decodeNewValues = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["newValues"]);
        if(!\SMMUtilities\CheckNecessityParam($decodeNewValues, array("name", "password", "priority"))) throw new Exception("Invalid parameter");

        //construct statement
        $insertKeyStatement = "";
        $insertValueStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateSeparatedStatement($decodeNewValues, 
        array("name" => new \SMMDatabaseStatement\ParamValueUserInput(PDO::PARAM_STR, "sm_name"),
            "password" => new \SMMDatabaseStatement\ParamValueUserInput(PDO::PARAM_STR, "sm_password"),
            "priority" => new \SMMDatabaseStatement\ParamValueUserInput(PDO::PARAM_INT, "sm_priority")),
        array("sm_registration" => new \SMMDatabaseStatement\ParamValueConstantInput(PDO::PARAM_INT, time()),
            "sm_salt" => new \SMMDatabaseStatement\ParamValueConstantInput(PDO::PARAM_INT, \SMMUtilities\GetRandomNumber()),
            "sm_token" => new \SMMDatabaseStatement\ParamValueConstantInput(PDO::PARAM_STR, ""),
            "sm_expireOn" => new \SMMDatabaseStatement\ParamValueConstantInput(PDO::PARAM_INT, 0)),
        $insertKeyStatement, $insertValueStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('INSERT user ' . $insertKeyStatement . ' VALUES ' . $insertValueStatement);
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());
        
    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "delete"))) {
        //rm, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("target"))) throw new Exception("Invalid parameter");
        $decodeTarget = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["target"]);
        if(count($decodeTarget) == 0) throw new Exception("Zero target is not allowed");

        //construct statement
        $whereStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateFilterStatement(array(), array(),
        array("sm_name" => new \SMMDatabaseStatement\ParamFilterConstantInput("=", PDO::PARAM_STR, $decodeTarget)),
        $whereStatement, $args);
        //bind param and execute
        $stmt = $db->conn->prepare('DELETE FROM user WHERE ' . $whereStatement);
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());
        
    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "update"))) {
        //update, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("target", "newValues"))) throw new Exception("Invalid parameter");
        $decodeTarget = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["target"]);
        $decodeNewValues = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["newValues"]);
        if(!\SMMUtilities\CheckOptionalParam($decodeNewValues, array("name", "password", "priority"), 1)) throw new Exception("Invalid parameter");
        if(count($decodeTarget) == 0) throw new Exception("Zero target is not allowed");

        //construct statement
        $setStatement = "";
        $whereStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateUpdateStatement($decodeNewValues,
        array("password" => new \SMMDatabaseStatement\ParamValueUserInput(PDO::PARAM_STR, "sm_password"),
            "priority" => new \SMMDatabaseStatement\ParamValueUserInput(PDO::PARAM_INT, "sm_priority"),
            "expireOn" => new \SMMDatabaseStatement\ParamValueUserInput(PDO::PARAM_INT, "sm_expireOn")),
        array(), $setStatement, $args);
        \SMMDatabaseStatement\GenerateFilterStatement(array(), array(),
        array("sm_name" => new \SMMDatabaseStatement\ParamFilterConstantInput("=", PDO::PARAM_STR, $decodeTarget)),
        $whereStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('UPDATE user SET ' . $setStatement . ' WHERE ' . $whereStatement);
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
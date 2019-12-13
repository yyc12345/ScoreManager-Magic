<?php

require_once "database.php";
require_once "utilities.php";
require_once "datastructure.php";

try {
    $db = new database();
    $db->lockdb();

    //confirm token and permission
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("token"))) throw new Exception("Invalid parameter");
    if(!$db->checkToken($_POST["token"])) throw new Exception("Invalid token");
    if(!(\SMMUtilities\CheckPriority($db->getPriority($_POST["token"]), \SMMDataStructure\EnumUserPriority::admin))) throw new Exception("No permission");

    if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "query"))) {
        //query, check param
        if (!\SMMUtilities\CheckNecessityParam($_POST, array("filterRules", "neededReturn"))) throw new Exception("Invalid parameter");
        //decode filter rules and needed return
        $decodeFilter = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["filterRules"]);
        $decodeNeeded = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["neededReturn"]);

        //contruct statement
        $whereStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateFieldFilterStatement($decodeFilter, array(
            "name" => new \SMMDatabaseStatement\ParamFilterUserInput('LIKE', PDO::PARAM_STR, "sm_name")
        ), $whereStatement, $args);

        $selectStatemnt = \SMMDatabaseStatement\GenerateFieldStatement($decodeNeeded,
        array("sm_name", "sm_password", "sm_registration", "sm_priority", "sm_salt", "sm_token", "sm_expireOn"));

        //bind param and execute
        $stmt = $db->conn->prepare('SELECT ' . $selectStatemnt . ' FROM user' . ( $whereStatement == "" ? "" : " WHERE " . $whereStatement));
        for($i = 0; $i<count($args) ; $i++) 
            $stmt->bindParam($i+1, $args[$i]->paramValue, $args[$i]->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", $stmt->fetchAll(PDO::FETCH_ASSOC)));

    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "add"))) {
        //add, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("newValues"))) throw new Exception("Invalid parameter");
        $decodeNewValues = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["filterRules"]);
        if(!\SMMUtilities\CheckNecessityParam($decodeNewValues, array("name", "password", "priority"))) throw new Exception("Invalid parameter");

        //construct statement
        $insertKeyStatement = "";
        $insertValueStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateSeparatedStatement($decodeNewValues, 
        array("name" => new \SMMDatabaseStatement\ParamSeperatedUserInput(PDO::PARAM_INT, "sm_name"),
            "password" => new \SMMDatabaseStatement\ParamSeperatedUserInput(PDO::PARAM_STR, "sm_password"),
            "priority" => new \SMMDatabaseStatement\ParamSeperatedUserInput(PDO::PARAM_INT, "sm_priority")),
        array("sm_registration" => new \SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_INT, time()),
            "sm_salt" => new \SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_INT, \SMMUtilities\GetRandomNumber()),
            "sm_token" => new \SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_STR, ""),
            "sm_expireOn" => new \SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_INT, 0)),
        $insertKeyStatement, $insertValueStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('INSERT user ' . $insertKeyStatement . ' VALUES ' . $insertValueStatement);
        for($i = 0; $i<count($args); $i++) 
            $stmt->bindParam($i+1, $args[$i]->paramValue, $args[$i]->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());
        
    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "delete"))) {
        //rm, check param


        
    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "update"))) {
        //update, check param


        
    } else throw new Exception("Invalid parameter");

    $db->unlockdb();
    $db = NULL;
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
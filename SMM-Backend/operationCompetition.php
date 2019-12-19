<?php
use function SMMUtilities\GetRandomCDK;

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

        //pre detect name where statement. query participant
        $constant = array();
        if (array_key_exists("name", $decodeFilter)) {
            $preWhereStatement = "";
            $args = array();
            SMMDatabaseStatement\GenerateFilterStatement(array(), array(),
            array("sm_participant" => new SMMDatabaseStatement\ParamFilterConstantInput('=', PDO::PARAM_STR, $decodeFilter["name"])),
            $preWhereStatement, $args);
            
            $prestmt = $db->conn->prepare('SELECT sm_id FROM competitionParticipant WHERE ' . $preWhereStatement);
            $prestmt->execute($args);

            $constant["sm_id"] = $prestmt->fetchAll(PDO::FETCH_COLUMN, 0);
        }
        
        //construct statement
        $whereStatement = "";
        $args = array();
        SMMDatabaseStatement\GenerateFilterStatement($decodeFilter,
        array("id" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_INT, "sm_id"),
            "name" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_INT, "sm_id"),
            "startDate" => new \SMMDatabaseStatement\ParamFilterUserInput('>', PDO::PARAM_INT, "sm_startDate"),
            "endDate" => new \SMMDatabaseStatement\ParamFilterUserInput('<', PDO::PARAM_INT, "sm_endDate"),
            "cdk" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_STR, "sm_cdk"),
            "map" => new \SMMDatabaseStatement\ParamFilterUserInput('=', PDO::PARAM_STR, "sm_map")),
        $constant, $whereStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('SELECT * FROM competition' . ($whereStatement == "" ? "" : " WHERE " . $whereStatement));
        for($i =0;$i<count($args); $i++)
            $stmt->bindParam($i+1, $args[$i]->paramValue, $args[$i]->paramSQLType);
        $stmt->execute();

        $finalData = $stmt->fetchAll(PDO::FETCH_ASSOC);

        //try detect sm_participant field
        $finalId = array();
        foreach($finalData as $i) $finalId[] = $i["sm_id"];

        $idWhereStatement = "";
        $args = array();
        SMMDatabaseStatement\GenerateFilterStatement(array(), array(), 
        array("sm_id" => new \SMMDatabaseStatement\ParamFilterConstantInput('=', PDO::PARAM_INT, $finalId)),
        $idWhereStatement, $args);

        $stmt2 = $db->conn->prepare('SELECT sm_id, sm_participant FROM competitionParticipant WHERE ' . $idWhereStatement);
        for($i =0;$i<count($args); $i++)
            $stmt2->bindParam($i+1, $args[$i]->paramValue, $args[$i]->paramSQLType);
        $stmt2->execute();

        $participant = array();
        foreach($stmt2->fetchAll(PDO::FETCH_GROUP) as $key=> $value) {
            $cache = array();
            foreach($value as $i) $cache[] = $i["sm_participant"];
            $participant[$key] = $cache;
        }

        //bind correspond output
        foreach($finalData as &$i) 
            $i["sm_participant"] = $participant[$i["sm_id"]];

            echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", $finalData));

    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "add"))) {
        //add, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("newValues"))) throw new Exception("Invalid parameter");
        $decodeNewValues = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["newValues"]);
        if(!\SMMUtilities\CheckNecessityParam($decodeNewValues, array("startDate", "endDate", "judgeEndDate", "participant"))) throw new Exception("Invalid parameter");
        $decodedParticipant = SMMUtilities\AdvancedJsonArrayDecoder($decodeNewValues["participant"]);

        //construct main table statement
        $insertKeyStatement = "";
        $insertValueStatement = "";
        $args = array();
        SMMDatabaseStatement\GenerateSeparatedStatement($decodeNewValues,
        array("startDate" => new SMMDatabaseStatement\ParamSeperatedUserInput(PDO::PARAM_INT, "sm_startDate"),
            "endDate" => new SMMDatabaseStatement\ParamSeperatedUserInput(PDO::PARAM_INT, "sm_endDate"),
            "judgeEndDate" => new SMMDatabaseStatement\ParamSeperatedUserInput(PDO::PARAM_INT, "sm_judgeEndDate")),
        array("sm_result" => new SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_STR, ""),
            "sm_map" => new SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_STR, ""),
            "sm_banMap" => new SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_STR, "[]"),
            "sm_cdk" => new SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_STR, GetRandomCDK()),
            "sm_winner" => new SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_STR, "")),
        $insertKeyStatement, $insertValueStatement, $args);

        $stmt = $db->conn->prepare('INSERT competition ' . $insertKeyStatement . ' VALUES ' . $insertValueStatement);
        foreach($args as $key=>$value)
            $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        //construct assi table statement
        $lastID = PDO::lastInsertId();
        foreach($decodedParticipant as $player) {
            $insertKeyStatement = "";
            $insertValueStatement = "";
            $args = array();
            SMMDatabaseStatement\GenerateSeparatedStatement(array(), array(),
            array("sm_id" => new SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_INT, $lastID),
                "sm_participant" => new SMMDatabaseStatement\ParamSeperatedConstantInput(PDO::PARAM_STR, $player)),
            $insertKeyStatement, $insertValueStatement, $args);

            $stmt2 = $db->conn->prepare('INSERT competitionParticipant ' . $insertKeyStatement . ' VALUES ' . $insertValueStatement);
            foreach($args as $key=>$value) $stmt2->bindParam($key+1, $value->paramValue, $value->paramSQLType);
            $stmt2->execute();
        }

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
        array("sm_id" => new \SMMDatabaseStatement\ParamFilterConstantInput("=", PDO::PARAM_STR, $decodeTarget)),
        $whereStatement, $args);

        $stmt = $db->conn->prepare('DELETE FROM competition WHERE ' . $whereStatement);
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        $stmt2 = $db->conn->prepare('DELETE FROM competitionParticipant WHERE ' . $whereStatement);
        foreach($args as $key=>$value) $stmt2->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt2->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());

    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "update"))) {
        //update, check param
        if(!\SMMUtilities\CheckNecessityParam($_POST, array("target", "newValues"))) throw new Exception("Invalid parameter");
        $decodeNewValues = \SMMUtilities\AdvancedJsonArrayDecoder($_POST["newValues"]);
        if(!\SMMUtilities\CheckOptionalParam($decodeNewValues, array("result", "map", "banMap", "winner"), 1)) throw new Exception("Invalid parameter");

        //construct statement
        $setStatement = "";
        $whereStatement = "";
        $args = array();
        \SMMDatabaseStatement\GenerateFilterStatement($decodeNewValues,
        array("result" => new \SMMDatabaseStatement\ParamFilterUserInput("=", PDO::PARAM_STR, "sm_result"),
            "map" => new \SMMDatabaseStatement\ParamFilterUserInput("=", PDO::PARAM_STR, "sm_map"),
            "banMap" => new \SMMDatabaseStatement\ParamFilterUserInput("=", PDO::PARAM_STR, "sm_banMap"),
            "winner" => new \SMMDatabaseStatement\ParamFilterUserInput("=", PDO::PARAM_STR, "sm_winner")),
        array(), $setStatement, $args);
        \SMMDatabaseStatement\GenerateFilterStatement(array(), array(),
        array("sm_name" => new \SMMDatabaseStatement\ParamFilterConstantInput("=", PDO::PARAM_STR, $_POST["target"])),
        $whereStatement, $args);

        //bind param and execute
        $stmt = $db->conn->prepare('UPDATE user SET ' . $setStatement . ' WHERE ' . $whereStatement);
        foreach($args as $key=>$value) $stmt->bindParam($key+1, $value->paramValue, $value->paramSQLType);
        $stmt->execute();

        echo json_encode(\SMMUtilities\GetUniversalReturn());
        
    } else throw new Exception("Invalid parameter");
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
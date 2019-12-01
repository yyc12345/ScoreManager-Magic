<?php

require_once "database.php";
require_once "utilities.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("name", "hash"))) throw new Exception("Invalid parameter");

    $db = new database();
    $db->lockdb();
    if(!$db->checkUser($_POST["name"])) throw new Exception("Invalid user name");

    //compute correct hash
    $stmt = $db->conn->prepare("SELECT sm_password, sm_salt FROM user WHERE sm_name = ?");
    $stmt->bindParam(1, $_POST["name"], PDO::PARAM_STR);
    $stmt->execute();
    $data = $stmt->fetch(PDO::FETCH_ASSOC);
    $rnd = $data["sm_salt"];
    settype($rnd, "string");
    $compare = hash("sha256", $data["sm_password"] . $rnd);

    //compare hash
    if ($compare != $_POST["hash"]) throw new Exception("Fail to auth login");

    //generate token
    $rnd = \SMMUtilities\GetRandomNumber();
    settype($rnd, "string");
    $token = hash("sha256", $data["sm_name"] . $rnd);
    $expireOn = \SMMUtilities\DateAddDays(time(), 1);
    //upload token
    $stmt2 = $db->conn->prepare("UPDATE user SET sm_token = ?, sm_expireOn = ? WHERE sm_name = ?");
    $stmt2->bindParam(1, $token, PDO::PARAM_STR);
    $stmt2->bindParam(2, $expireOn, PDO::PARAM_INT);
    $stmt2->bindParam(3, $_POST["name"], PDO::PARAM_STR);
    $stmt2->execute();

    //get priority again
    $priority = $db->getPriority($token);

    $db->unlockdb();
    $db = NULL;

    echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", array("token" => $token, "priority" => $priority)));
    
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}


?>
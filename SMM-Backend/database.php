<?php

require_once "config.php";
require_once "utilities.php";

class database {

    var $conn;

    function __construct() {
        global $GLOBAL_CONFIG;
        $this->conn = new PDO("mysql:host=" . $GLOBAL_CONFIG["database"]["url"] . ":" . $GLOBAL_CONFIG["database"]["port"] . ";dbname=" . $GLOBAL_CONFIG["database"]["db"], $GLOBAL_CONFIG["database"]["user"], $GLOBAL_CONFIG["database"]["password"]);
        $this->conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        $this->conn->exec("set names utf8");
    }

    function __destruct() {
        $this->conn = NULL;
    }

    public function lockdb() {
        $this->conn->exec("LOCK TABLE user WRITE, record WRITE, map WRITE, mapPool WRITE, tournament WRITE, participant WRITE, competition WRITE, competitionParticipant WRITE");
    }

    public function unlockdb() {
        $this->conn->exec("UNLOCK TABLES");
    }

    //universal
    public function getPriority($token) {
        $stmt = $this->conn->prepare("SELECT * FROM user WHERE sm_token = ?");
        $stmt->bindParam(1, $token, PDO::PARAM_STR);
        $stmt->execute();
        $data = $stmt->fetch(PDO::FETCH_ASSOC);
        return $data["sm_priority"];
    }

    public function checkToken($token) {
        $stmt = $this->conn->prepare("SELECT * FROM user WHERE sm_token = ?");
        $stmt->bindParam(1, $token, PDO::PARAM_STR);
        $stmt->execute();

        return !(count($stmt->fetchAll()) == 0);
    }

    public function getUserFromToken($token) {
        $stmt = $this->conn->prepare("SELECT * FROM user WHERE sm_token = ?");
        $stmt->bindParam(1, $token, PDO::PARAM_STR);
        $stmt->execute();

        $data = $stmt->fetch(PDO::FETCH_ASSOC);
        return $data["sm_name"];
    }

    public function checkUser($name) {
        $stmt = $this->conn->prepare("SELECT * FROM user WHERE sm_name = ?");
        $stmt->bindParam(1, $name, PDO::PARAM_STR);
        $stmt->execute();

        return !(count($stmt->fetchAll()) == 0);
    }
    
}

?>
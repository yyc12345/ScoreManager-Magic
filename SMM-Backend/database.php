<?php

require_once "config.php";
require_once "utilities.php";

class database {

    private $conn;

    function __construct() {
        global $GLOBAL_CONFIG;
        $this->$conn = new PDO("mysql:host=" . $GLOBAL_CONFIG["database"]["url"] . ":" . $GLOBAL_CONFIG["database"]["port"] . ";dbname=" . $GLOBAL_CONFIG["database"]["db"], $GLOBAL_CONFIG["database"]["user"], $GLOBAL_CONFIG["database"]["password"]);
        $this->$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        $this->$conn->exec("set names utf8");
    }

    function __destruct() {
        $this->$conn = NULL;
    }

    //init
    public function init() {
        //set up table
        $this->$conn->exec("CREATE TABLE user (
            sm_name TEXT,
            sm_password VARCHAR(64),
            sm_registration BIGINT UNSIGNED,
            sm_priority TINYINT UNSIGNED,
            sm_salt INT,
            sm_token VARCHAR(64),
            sm_expireOn BIGINT UNSIGNED
            )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
        $this->$conn->exec("CREATE TABLE record (
            sm_name TEXT,
            
            sm_installedOn TINYINT,
            sm_map VARCHAR(64),
            
            sm_score INT,
            sm_srTime INT,
            
            sm_lifeUp INT,
            sm_lifeLost INT,
            sm_extraPoint INT,
            sm_subExtraPoint INT,
            sm_trafo INT,
            sm_checkpoint INT,
            sm_verify TINYINT UNSIGNED,
            sm_token TEXT,
            
            sm_localUTC BIGINT UNSIGNED,
            sm_serverUTC BIGINT UNSIGNED
            )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
        $this->$conn->exec("CREATE TABLE map (
            sm_name TEXT,
            sm_author TEXT,
            sm_hash VARCHAR(64)
            )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
        $this->$conn->exec("CREATE TABLE tournament (
            sm_tournament TEXT
            )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
        $this->$conn->exec("CREATE TABLE participant (
            sm_id TEXT,
            sm_type TINYINT UNSIGNED,
            sm_registration BIGINT UNSIGNED,
            sm_tournament TEXT
            )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
        $this->$conn->exec("CREATE TABLE competition (
            sm_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
            sm_red TEXT,
            sm_redRes BIGINT,
            sm_blue TEXT,
            sm_blueRes BIGINT,
            sm_startDate BIGINT,
            sm_endDate BIGINT,
            sm_map VARCHAR(64),
            sm_tournament TEXT,
            sm_winner TEXT,

            PRIMARY KEY ( sm_id )
            )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
    }

    public function lockdb() {
        $this->$conn->exec("LOCK TABLE user WRITE, record WRITE, map WRITE, tournament WRITE, participant WRITE, competition WRITE");
    }

    public function unlockdb() {
        $this->$conn->exec("UNLOCK TABLES");
    }

    //universal
    public function checkPermission($token, $expectPermission) {
        $stmt = $this->$conn->prepare("SELECT * FROM user WHERE sm_token = ?");
        $stmt->bindParam(1, $token, PDO::PARAM_STR);
        $stmt->execute();
        $data = $stmt->fetch(PDO::FETCH_ASSOC);
        if ($expectPermission <= $data["sm_priority"]) {
            return true;
        } else {
            return false;
        }
    }

    public function checkLogin($token) {
        $stmt = $this->$conn->prepare("SELECT * FROM user WHERE sm_token = ?");
        $stmt->bindParam(1, $token, PDO::PARAM_STR);
        $stmt->execute();

        return !(count($stmt->fetchAll()) == 0);
    }

    //user
    public function addUser($name, $password, $priority) {
        $stmt = $this->$conn->prepare("INSERT INTO user (sm_name, sm_password, sm_registration, sm_priority, sm_salt, sm_token, sm_expireOn)
VALUES (?, ?, ?, ?, 0, '', 0)");
        $stmt->bindParam(1, $name, PDO::PARAM_STR);
        $stmt->bindParam(2, $password, PDO::PARAM_STR);
        $stmt->bindParam(3, time(), PDO::PARAM_INT);
        $stmt->bindParam(4, $priority, PDO::PARAM_INT);
        $stmt->execute();
    }

    public function checkUser($name) {
        $stmt = $this->$conn->prepare("SELECT * FROM user WHERE sm_name = ?");
        $stmt->bindParam(1, $name, PDO::PARAM_STR);
        $stmt->execute();

        return !(count($stmt->fetchAll()) == 0);
    }
    
    public function generateSalt($user) {
        $rnd = mt_rand(0,6172748);
        $stmt = $this->$conn->prepare("UPDATE user SET sm_salt = ? WHERE sm_name = ?");
        $stmt->bindParam(1, $rnd, PDO::PARAM_INT);
        $stmt->bindParam(2, $user, PDO::PARAM_STR);
        $stmt->execute();
        return $rnd;
    }

    public function authUser($user, $hash) {
        //get data
        $stmt = $this->$conn->prepare("SELECT * FROM user WHERE sm_name = ?");
        $stmt->bindParam(1, $user, PDO::PARAM_STR);
        $stmt->execute();
        $data = $stmt->fetch(PDO::FETCH_ASSOC);
        $rnd = $data["sm_salt"];
        settype($rnd, "string");
        $compare = hash("sha256", $data["sm_password"] . $rnd);

        //compare
        if ($compare == $hash) {
            //generate token
            $rnd = mt_rand(0,6172748);
            settype($rnd, "string");
            $token = hash("sha256", $data["sm_name"] . $rnd);
            $expireOn = time() + 60 * 60 * 24;
            //upload token
            $stmt2 = $this->$conn->prepare("UPDATE user SET sm_token = ?, sm_expireOn = ? WHERE sm_name = ?");
            $stmt2->bindParam(1, $token, PDO::PARAM_STR);
            $stmt2->bindParam(2, $expireOn, PDO::PARAM_INT);
            $stmt2->bindParam(3, $user, PDO::PARAM_STR);
            $stmt2->execute();

            return $token;
        } else {
            return "";
        }
    }

    public function resetToken($token) {
        $rnd = mt_rand(0,6172748);
        $stmt = $this->$conn->prepare("UPDATE user SET sm_token = '', sm_expireOn = 0, sm_salt = ? WHERE sm_token = ?");
        $stmt->bindParam(1, $rnd, PDO::PARAM_INT);
        $stmt->bindParam(2, $token, PDO::PARAM_STR);
        $stmt->execute();
    }

    public function addSubmit($token, $installOn, $map, $score, $srTime, $lifeUp, $lifeLost, $extraPoint, $subExtraPoint, $trafo, $checkpoint, $verify ,$bsmToken, $localTime) {
        $serverTime = time();

        //query name
        $preStmt = $this->$conn->prepare("SELECT * FROM user WHERE sm_token = ?");
        $preStmt->bindParam(1, $token, PDO::PARAM_STR);
        $preStmt->execute();
        $data=$preStmt->fetch(PDO::FETCH_ASSOC);
        $user=$data["sm_name"];

        //submit
        $stmt = $this->$conn->prepare("INSERT INTO record (sm_name, sm_installOn, sm_map, sm_score, sm_srTime, sm_lifeUp, sm_lifeLost, sm_extraPoint, sm_subExtraPoint, sm_trafo, sm_checkpoint, sm_verify, sm_token, sm_localUTC, sm_serverUTC)
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
        $stmt->bindParam(1 ,$user , PDO::PARAM_STR);
        $stmt->bindParam(2 ,$installOn , PDO::PARAM_INT);
        $stmt->bindParam(3 ,$map , PDO::PARAM_STR);
        $stmt->bindParam(4 ,$score , PDO::PARAM_INT);
        $stmt->bindParam(5 ,$srTime , PDO::PARAM_INT);
        $stmt->bindParam(6 ,$lifeUp , PDO::PARAM_INT);
        $stmt->bindParam(7 ,$lifeLost , PDO::PARAM_INT);
        $stmt->bindParam(8 ,$extraPoint , PDO::PARAM_INT);
        $stmt->bindParam(9 ,$subExtraPoint , PDO::PARAM_INT);
        $stmt->bindParam(10 ,$trafo , PDO::PARAM_INT);
        $stmt->bindParam(11 ,$checkpoint , PDO::PARAM_INT);
        $stmt->bindParam(12 ,$verify , PDO::PARAM_INT);
        $stmt->bindParam(13 ,$bsmToken , PDO::PARAM_STR);
        $stmt->bindParam(14 ,$localTime , PDO::PARAM_INT);
        $stmt->bindParam(15 ,$serverTime , PDO::PARAM_INT);
        $stmt->execute();
    }

}

?>
<?php

require_once "database.php";

$INIT_ROOT_ACCOUNT = array(
    "user" => "root",
    "password" => "password"
);

//establish database
try {
    $db = new database();
    //set up table
    $db->conn->exec("CREATE TABLE user (
        sm_name TEXT,
        sm_password VARCHAR(64),
        sm_registration BIGINT UNSIGNED,
        sm_priority TINYINT UNSIGNED,
        sm_salt INT,
        sm_token VARCHAR(32),
        sm_expireOn BIGINT UNSIGNED
        );");
    $db->conn->exec("CREATE TABLE record (
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
    $db->conn->exec("CREATE TABLE map (
        sm_name TEXT,
        sm_i8n TEXT,
        sm_hash VARCHAR(64)
        )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
    $db->conn->exec("CREATE TABLE mapPool (
        sm_hash VARCHAR(64),
        sm_tournament TEXT
        )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
    $db->conn->exec("CREATE TABLE tournament (
        sm_tournament TEXT,
        sm_startDate BIGINT,
        sm_endDate BIGINT,
        sm_schedule LONGTEXT
        )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
    $db->conn->exec("CREATE TABLE participant (
        sm_user TEXT,
        sm_tournament TEXT
        )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
    $db->conn->exec("CREATE TABLE competition (
        sm_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
        sm_result TEXT,
        sm_startDate BIGINT,
        sm_endDate BIGINT,
        sm_judgeEndDate BIGINT,
        sm_map VARCHAR(64),
        sm_banMap TEXT,
        sm_cdk TEXT,
        sm_winner TEXT,
        
        PRIMARY KEY ( sm_id )
        )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
    $db->conn->exec("CREATE TABLE competitionParticipant (
        sm_id BIGINT UNSIGNED,
        sm_participant TEXT
        )ENGINE=InnoDB DEFAULT CHARSET=utf8;");
    
    //add init user
    $user_hash = hash("sha256", $INIT_ROOT_ACCOUNT["password"]);

    //set init user
    $srvTime = time();
    $rndNumber = \SMMUtilities\GetRandomNumber();
    $stmt = $db->conn->prepare("INSERT INTO user (sm_name, sm_password, sm_registration, sm_priority, sm_salt, sm_token, sm_expireOn) VALUES (?, ?, ?, 4, ?, '', 0)");
    $stmt->bindParam(1, $INIT_ROOT_ACCOUNT["user"], PDO::PARAM_STR);
    $stmt->bindParam(2, $user_hash, PDO::PARAM_STR);
    $stmt->bindParam(3, $srvTime, PDO::PARAM_INT);
    $stmt->bindParam(4, $rndNumber, PDO::PARAM_INT); //random salt to prevent try login
    $stmt->execute();

    $db = NULL;

    echo "Done";
    
} catch (Exception $e) {
    echo $e->getMessage();
}

?>
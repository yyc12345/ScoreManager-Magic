<?php

function GetUniversalReturn($successful = true, $err = "OK") {
    $res = array(
        "code" => $successful ? "200" : "400",
        "err" => $err
    );
    return $res;
}

function CheckParameter($ori, $sample) {
    foreach($sample as $x) {
        if (!array_key_exists($x, $ori)) {
            return false;
        }
    }
    return true;
}

function CheckPriority($priority, $wanted) {
    switch($wanted) {
        case "user":
            return (($priority & 1) == 1);
        case "live":
            return (($priority & 2) == 2);
        case "speedrun":
            return (($priority & 4) == 4);
        case "admin":
            return (($priority & 8) == 8);
        default:
            return false;
    }
}

?>
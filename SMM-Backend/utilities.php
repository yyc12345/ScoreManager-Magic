<?php

namespace SMMUtilities {

    require_once "datastructure.php";

    function GetUniversalReturn($successful = true, $err = "OK", $data = "")
    {
        $res = array(
            "code" => $successful ? 200 : 400,
            "err" => $err,
            "data" => $data
        );
        return $res;
    }

    function CheckPriority($priority, $wanted)
    {
        switch ($wanted) {
            case "user":
                return (($priority & \SMMDataStructure\EnumUserPriority::user) == \SMMDataStructure\EnumUserPriority::user);
            case "live":
                return (($priority & \SMMDataStructure\EnumUserPriority::live) == \SMMDataStructure\EnumUserPriority::live);
            case "speedrun":
                return (($priority & \SMMDataStructure\EnumUserPriority::speedrun) == \SMMDataStructure\EnumUserPriority::speedrun);
            case "admin":
                return (($priority & \SMMDataStructure\EnumUserPriority::admin) == \SMMDataStructure\EnumUserPriority::admin);
            default:
                return false;
        }
    }

    //========================================================param checker
    function CheckNecessityParam($target, $necessity) {
        //check necessity
        foreach ($necessity as $x) {
            if (!array_key_exists($x, $target)) {
                return false;
            }
        }
        return true;
    }

    function CheckHardcodeParam($target, $hardcode) {
        //check hardcode
        foreach ($hardcode as $key => $value) {
            if (!array_key_exists($key, $target)) {
                return false;
            }
            if ($target[$key] != $value) {
                return false;
            }
        }
        return true;
    }

    function CheckOptionalParam($target, $optitional, $optitionalMininumLimit) {
        //check optitional
        $count = 0;
        foreach ($optitional as $x) {
            if (array_key_exists($x, $target)) {
                $count+=1;
            }
        }
        if ($count<$optitionalMininumLimit) {
            return false;
        }
    
        return true;
    }
}

?>
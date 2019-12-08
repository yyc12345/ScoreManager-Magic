<?php

namespace SMMUtilities {

    function GetUniversalReturn($successful = true, $err = "OK", $data = "") {
        $res = array(
            "code" => $successful ? 200 : 400,
            "err" => $err,
            "data" => $data
        );
        return $res;
    }

    function CheckPriority($priority, $wanted) {
        return (($priority & $wanted) == $wanted);
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
            if (!array_key_exists($key, $target) && $target[$x] != "") {
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
            if (array_key_exists($x, $target) && $target[$x] != "") {
                $count+=1;
            }
        }
        if ($count<$optitionalMininumLimit) {
            return false;
        }
    
        return true;
    }

    function CheckAssocParam($target, $assco, $assocParam) {
        if (CheckNecessityParam($target, $assoc)) return CheckNecessityParam($assocParam);
        else return true;
    }


    //========================================================misc func
    function GetRandomNumber() {
        //need security random number generator
        return mt_rand(0,6172748);
    }

    function DateAddDays($date, $days) {
        return $date + $days * 60 * 60 * 24;
    }

    function AdvancedJsonArrayDecoder($jsonstr) {
        $result = json_decode($jsonstr, true);
        if(!is_array($result)) throw new Exception("Fail to decode json");
        return $result;
    }

}

namespace SMMUtilities\DatabaseStatementGenerator {

    class ParamPropetry {
        var $paramLikeMode;
        var $paramSQLType;
        var $paramDatabaseField;

        function __construct($pLike, $pType, $pField) {
            $this->paramLikeMode = $pLike;
            $this->paramSQLType = $pType;
            $this->paramDatabaseField = $pField;
        }
    }

    class ParamOutProperty {
        var $paramValue;
        var $paramSQLType;

        function __construct($pValue, $pType) {
            $this->paramValue = $pValue;
            $this->paramSQLType = $pType;
        }
    }

    //========================================================sql statement generator
    function GenerateFieldFilterStatement($data, $criteria, &$outStatement, &$outArguments) {
        $usefulField = array_intersect(array_keys($data), array_keys($criteria));
        if(count($usefulField) != 0) {
            $cache = array();
            foreach($usefulField as $i) {
                if(is_array($data[$i])) {
                    //param is array
                    $cache2 = array();

                    foreach($data[$i] as $ii) {
                        if ($criteria[$i]->paramLikeMode) $cache2[] = ' (' . $criteria[$i]->paramDatabaseField . ' LIKE ' . '?) '; //use like
                        else $cache2[] = ' (' . $criteria[$i]->paramDatabaseField . ' = ' . '?) '; //use equal

                        $outArguments[] = new ParamOutProperty($ii, $criteria[$i]->paramSQLType);
                    }

                    $cache[] = ' (' . join(' || ', $cache2) . ') ';
                } else {
                    if ($criteria[$i]->paramLikeMode) $cache[] = ' (' . $criteria[$i]->paramDatabaseField . ' LIKE ' . '?) '; //use like
                    else $cache[] = ' (' . $criteria[$i]->paramDatabaseField . ' = ' . '?) '; //use equal

                    $outArguments[] = new ParamOutProperty($data[$i], $criteria[$i]->paramSQLType);
                }
                
            }
            $outStatement = ' (' . join(' && ', $cache) . ') ';
        } else $outStatement = "";
    }

    function GenerateFieldStatement($data, $criteria) {
        $usefulField = array_intersect($data, $criteria);
        if(count($usefulField) == 0) throw new Exception("Selected colums should not be zero");
        return ' SELECT ' . join(', ', $usefulField) . ' ';
    }

}

?>
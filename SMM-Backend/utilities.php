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

    class ParamFilterUserInput {
        var $paramLikeMode;
        var $paramSQLType;
        var $paramDatabaseField;

        function __construct($pLike, $pType, $pField) {
            $this->paramLikeMode = $pLike;
            $this->paramSQLType = $pType;
            $this->paramDatabaseField = $pField;
        }
    }

    class ParamSeperatedUserInput {
        var $paramSQLType;
        var $paramDatabaseField;

        function __construct($pType, $pField) {
            $this->paramSQLType = $pType;
            $this->paramDatabaseField = $pField;
        }
    }

    class ParamFilterConstantInput {
        var $paramValue;
        var $paramLikeMode;
        var $paramSQLType;

        function __construct($pLike, $pType, $pValue) {
            $this->paramLikeMode = $pLike;
            $this->paramSQLType = $pType;
            $this->paramValue = $pValue;
        }
    }

    class ParamSeperatedConstantInput {
        var $paramValue;
        var $paramSQLType;

        function __construct($pType, $pValue) {
            $this->paramSQLType = $pType;
            $this->paramValue = $pValue;
        }
    }

    class ParamOutput {
        var $paramValue;
        var $paramSQLType;

        function __construct($pValue, $pType) {
            $this->paramValue = $pValue;
            $this->paramSQLType = $pType;
        }
    }


    //========================================================sql statement generator
    //criteria:
    //    key: string(correspond with input field name)
    //    value: ParamFilterUserInput(indicate like mode, corresponding database field and input type)
    //data:
    //    key: string(correspond with input field name)
    //    value: string(real data)
    //constantValue:
    //    key: string(correspond with database field name)
    //    value: ParamFilterConstantInput(indicate like mode, value and input type)
    function GenerateFieldFilterStatement($data, $criteria, $constantValue, &$outStatement, &$outArguments) {
        $usefulField = array_intersect(array_keys($data), array_keys($criteria));
        if(count($usefulField) != 0) {
            $cache = array();
            //input user data
            foreach($usefulField as $i) {
                if(is_array($data[$i])) {
                    //param is array
                    $cache2 = array();

                    foreach($data[$i] as $ii) {
                        if ($criteria[$i]->paramLikeMode) $cache2[] = ' (' . $criteria[$i]->paramDatabaseField . ' LIKE ' . '?) '; //use like
                        else $cache2[] = ' (' . $criteria[$i]->paramDatabaseField . ' = ' . '?) '; //use equal

                        $outArguments[] = new ParamOutput($ii, $criteria[$i]->paramSQLType);
                    }

                    $cache[] = ' (' . join(' || ', $cache2) . ') ';
                } else {
                    if ($criteria[$i]->paramLikeMode) $cache[] = ' (' . $criteria[$i]->paramDatabaseField . ' LIKE ' . '?) '; //use like
                    else $cache[] = ' (' . $criteria[$i]->paramDatabaseField . ' = ' . '?) '; //use equal

                    $outArguments[] = new ParamOutput($data[$i], $criteria[$i]->paramSQLType);
                }
                
            }
            //input constant data
            foreach($constantValue as $key => $value) {
                if (is_array($value)) {
                    //value is array
                    $cache2 = array();

                    foreach($value as $ii) {
                        if ($value->paramLikeMode) $cache2[] = ' (' . $key . ' LIKE ' . '?) '; //use like
                        else $cache2[] = ' (' . $key . ' = ' . '?) '; //use equal

                        $outArguments[] = new ParamOutput($ii, $value->paramSQLType);
                    }

                    $cache[] = ' (' . join(' || ', $cache2) . ') ';
                } else {
                    if ($value->paramLikeMode) $cache[] = ' (' . $key . ' LIKE ' . '?) '; //use like
                    else $cache[] = ' (' . $key . ' = ' . '?) '; //use equal

                    $outArguments[] = new ParamOutput($value->paramValue, $value->paramSQLType);
                }
            }
            $outStatement = ' (' . join(' && ', $cache) . ') ';
        } else $outStatement = "";
    }

    //criteria:
    //    item: string(correspond with database field name, from pre-definition)
    //data:
    //    item: string(correspond with database field name, from user input)
    function GenerateFieldStatement($data, $criteria) {
        $usefulField = array_intersect($data, $criteria);
        if(count($usefulField) == 0) throw new Exception("Selected colums should not be zero");
        return ' SELECT ' . join(', ', $usefulField) . ' ';
    }

    //criteria:
    //    key: string(correspond with input field name)
    //    value: ParamSeperatedUserInput
    //data:
    //    key: string(correspond with input field name)
    //    value: string(real data)
    //constantValue:
    //    key: string(correspond with database field name)
    //    value: ParamSeperatedConstantInput(indicate like mode, value and input type)
    function GenerateSeparatedStatement($data, $criteria, $constantValue, &$outKeyStatement, &$outValueStatement, &$outArguments) {
        $usefulField = array_intersect(array_key($data), array($criteria));
        if(count($usefulField) != 0){
            $cache = array();
            foreach($usefulField as $i) {
                $cache[] = $criteria[$i]->paramDatabaseField;
                $outArguments[] = new ParamOutput($data[$i], $criteria[$i]->paramSQLType);
            }
            foreach($constantValue as $key => $value) {
                $cache[] = $key;
                $outArguments[] = new ParamOutput($value->paramValue, $value->paramSQLType);
            }

            $outKeyStatement = ' (' . join(', ', $cache) . ') ';
            $outValueStatement = ' (' . join(', ', array_fill(0, count($usefulField) + count($constantValue), '?')) . ') ';
        } else {
            $outKeyStatement = "";
            $outValueStatement = "";
        }
    }

}

?>
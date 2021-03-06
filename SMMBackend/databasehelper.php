<?php

namespace SMMDataStructure {

    class EnumUserPriority {
        const user = 1;
        const live = 2;
        const speedrun = 4;
        const admin = 8;
    }

}

namespace SMMDatabaseStatement {

    class ParamFilterUserInput {
        var $paramCompareSymbol;
        var $paramSQLType;
        var $paramDatabaseField;

        function __construct($pLike, $pType, $pField) {
            $this->paramCompareSymbol = $pLike;
            $this->paramSQLType = $pType;
            $this->paramDatabaseField = $pField;
        }
    }

    class ParamValueUserInput {
        var $paramSQLType;
        var $paramDatabaseField;

        function __construct($pType, $pField) {
            $this->paramSQLType = $pType;
            $this->paramDatabaseField = $pField;
        }
    }

    class ParamFilterConstantInput {
        var $paramValue;
        var $paramCompareSymbol;
        var $paramSQLType;

        function __construct($pLike, $pType, $pValue) {
            $this->paramCompareSymbol = $pLike;
            $this->paramSQLType = $pType;
            $this->paramValue = $pValue;
        }
    }

    class ParamValueConstantInput {
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
    function GenerateFilterStatement($data, $criteria, $constantValue, &$outStatement, &$outArguments) {
        $usefulField = array_intersect(array_keys($data), array_keys($criteria));
        if(count($usefulField) != 0 || count($constantValue) != 0) {
            $cache = array();
            //input user data
            foreach($usefulField as $i) {
                if(is_array($data[$i])) {
                    //param is array
                    $cache2 = array();

                    foreach($data[$i] as $ii) {
                        $cache2[] = ' ' . $criteria[$i]->paramDatabaseField . ' ' . $criteria[$i]->paramCompareSymbol .' ? ';
                        $outArguments[] = new ParamOutput($ii, $criteria[$i]->paramSQLType);
                    }

                    if (count($cache2) == 1) $cache[] = $cache2[0];
                    else $cache[] = ' (' . join(' || ', $cache2) . ') ';
                } else {
                    $cache[] = ' ' . $criteria[$i]->paramDatabaseField . ' ' . $criteria[$i]->paramCompareSymbol . ' ? ';

                    $outArguments[] = new ParamOutput($data[$i], $criteria[$i]->paramSQLType);
                }
                
            }
            //input constant data
            foreach($constantValue as $key => $value) {
                if (is_array($value->paramValue)) {
                    //value is array
                    $cache2 = array();

                    foreach($value->paramValue as $ii) {
                        $cache2[] = ' ' . $key . ' ' . $value->paramCompareSymbol .' ? ';
                        $outArguments[] = new ParamOutput($ii, $value->paramSQLType);
                    }

                    if (count($cache2) == 1) $cache[] = $cache2[0];
                    else $cache[] = ' (' . join(' || ', $cache2) . ') ';
                } else {
                    $cache[] = ' ' . $key . ' ' . $value->paramCompareSymbol .' ? ';
                    $outArguments[] = new ParamOutput($value->paramValue, $value->paramSQLType);
                }
            }
            
            if(count($cache) == 1) $outStatement = $cache[0];
            else $outStatement = ' (' . join(' && ', $cache) . ') ';
        } else $outStatement = "";
    }

    //criteria:
    //    item: string(correspond with database field name, from pre-definition)
    //data:
    //    item: string(correspond with database field name, from user input)
    //constantValue:
    //    item: string(correspond with database field name, from constant. it will be set as the head of the entire statement)
    function GenerateFieldStatement($data, $criteria, $constantValue) {
        $usefulField = array_intersect($data, $criteria);
        if(count($usefulField) == 0 && count($constantValue) == 0) throw new Exception("Selected colums should not be zero");
        return ' SELECT ' . join(', ', array_merge($constantValue, $usefulField)) . ' ';
    }

    //criteria:
    //    key: string(correspond with input field name)
    //    value: ParamValueUserInput
    //data:
    //    key: string(correspond with input field name)
    //    value: string(real data)
    //constantValue:
    //    key: string(correspond with database field name)
    //    value: ParamValueConstantInput(indicate like mode, value and input type)
    function GenerateSeparatedStatement($data, $criteria, $constantValue, &$outKeyStatement, &$outValueStatement, &$outArguments) {
        $usefulField = array_intersect(array_keys($data), array_keys($criteria));
        if(count($usefulField) != 0 || count($constantValue) != 0){
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

    //criteria:
    //    key: string(correspond with input field name)
    //    value: ParamValueUserInput
    //data:
    //    key: string(correspond with input field name)
    //    value: string(real data)
    //constantValue:
    //    key: string(correspond with database field name)
    //    value: ParamValueConstantInput(indicate like mode, value and input type)
    function GenerateUpdateStatement($data, $criteria, $constantValue, &$outStatement, &$outArguments) {
        $usefulField = array_intersect(array_keys($data), array_keys($criteria));
        if(count($usefulField) != 0 || count($constantValue) != 0){
            $cache = array();

            foreach($usefulField as $i) {
                $cache[] = $criteria[$i]->paramDatabaseField . ' = ? ';
                $outArguments[] = new ParamOutput($data[$i], $criteria[$i]->paramSQLType);
            }
            foreach($constantValue as $key => $value) {
                $cache[] = $key . ' = ? ';
                $outArguments[] = new ParamOutput($value->paramValue, $value->paramSQLType);
            }

            $outStatement = ' ' . join(', ', $cache) . ' ';
        } else $outStatement = "";
    }
}


?>
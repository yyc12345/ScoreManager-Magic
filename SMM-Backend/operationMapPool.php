<?php

require_once "database.php";
require_once "utilities.php";
require_once "datastructure.php";

try {
    if (!\SMMUtilities\CheckNecessityParam($_POST, array("name"))) throw new Exception("Invalid parameter");

    if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "query"))) {
        //query



    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "add"))) {
        //add


        
    } else if (\SMMUtilities\CheckHardcodeParam($_POST, array("method" => "delete"))) {
        //rm


        
    } else throw new Exception("Invalid parameter");
} catch (Exception $e) {
    echo json_encode(\SMMUtilities\GetUniversalReturn(false, $e->getMessage()));
}

?>
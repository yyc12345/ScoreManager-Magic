<?php

require_once "utilities.php";
require_once "config.php";

global $GLOBAL_CONFIG;
echo json_encode(\SMMUtilities\GetUniversalReturn(true, "OK", array("ver" => $GLOBAL_CONFIG["version"])));

?>
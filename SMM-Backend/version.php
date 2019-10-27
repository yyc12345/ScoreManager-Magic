<?php

require_once "utilities.php";
require_once "config.php";

echo json_encode(GetUniversalReturn(true, "OK", array("ver" => $GLOBAL_CONFIG["version"])));

?>
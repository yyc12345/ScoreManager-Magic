<?php

namespace SMMDataStructure {

    class EnumUserPriority {
        const user = 1;
        const live = 2;
        const speedrun = 4;
        const admin = 8;
    }

}

namespace SMMDataStructure\DatabaseField {

    function GetTableUser() {
        return array("sm_name", 
                    "sm_password",
                    "sm_registration",
                    "sm_priority",
                    "sm_salt",
                    "sm_token",
                    "sm_expireOn");
    }

}

?>
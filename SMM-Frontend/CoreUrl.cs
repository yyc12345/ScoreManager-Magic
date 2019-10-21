using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMM_Frontend {
    internal static class CoreUrl {

        internal static string CoreURL = "http://yyc.bkt.moe:6666/";

        internal static string Salt => CoreURL + "salt.php";

        internal static string Login => CoreURL + "login.php";

        internal static string Logout => CoreURL + "logout.php";

    }
}

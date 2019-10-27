using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMM_Frontend {
    internal static class CoreUrl {

        internal static string RequestBaseUrl = "http://yyc.bkt.moe:6666/";

        internal static string Version => RequestBaseUrl + "version.php";

        internal static string Salt => RequestBaseUrl + "salt.php";

        internal static string Login => RequestBaseUrl + "login.php";

        internal static string Logout => RequestBaseUrl + "logout.php";

        internal static string Submit => RequestBaseUrl + "submit.php";

    }
}

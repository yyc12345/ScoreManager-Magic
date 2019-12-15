using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMMLib.Net {
    internal static class CoreUrl {

        internal static string RequestBaseUrl = "http://yyc.bkt.moe:6666/";

        internal static string Version => RequestBaseUrl + "version.php";
        internal static string Salt => RequestBaseUrl + "salt.php";
        internal static string Login => RequestBaseUrl + "login.php";
        internal static string Logout => RequestBaseUrl + "logout.php";
        internal static string Submit => RequestBaseUrl + "submit.php";
        internal static string ChangePassword => RequestBaseUrl + "changePassword.php";
        internal static string GetFutureCompetition => RequestBaseUrl + "getFutureCompetition.php";
        internal static string GetMapName => RequestBaseUrl + "getMapName.php";
        internal static string GetTournament => RequestBaseUrl + "getTournament.php";
        internal static string RegisterTournament => RequestBaseUrl + "registerTournament.php";
        internal static string OperationUser => RequestBaseUrl + "operationUser.php";
        internal static string OperationCompetition => RequestBaseUrl + "operationCompetition.php";
        internal static string OperationRecord => RequestBaseUrl + "operationRecord.php";
        internal static string OperationTournament => RequestBaseUrl + "operationTournament.php";
        internal static string OperationRegistry => RequestBaseUrl + "operationRegistry.php";
        internal static string OperationMapPool => RequestBaseUrl + "operationMapPool.php";
        internal static string OperationMap => RequestBaseUrl + "operationMap.php";

    }
}

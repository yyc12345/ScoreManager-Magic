using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMMLib.Utilities;
using ScoreManager_Magic.Core;

namespace ScoreManager_Magic {
    public static class SharedModule {

        public static ConfigManager configManager = new ConfigManager("sccoremanager-magic.cfg", new Dictionary<string, string>() {
            {"BallancePath", Information.WorkPath.Enter("Ballance").Path },
#if DEBUG
            {"Server", "http://yyc.bkt.moe:6666/" },
#else
            {"Server", "http://yyc.bkt.moe:7777/" },
#endif
            {"RememberUser", "0"},
            {"RememberPassword", "0"},
            {"RememberedUser", ""},
            {"RememberedPassword", ""}
        });

        public static Anticheat anticheat = new Anticheat();
        public static NemosWatcher nemosWatcher = new NemosWatcher();
        public static LogManager logSystem = new LogManager(Information.WorkPath.Enter("scoremanager-magic.log").Path);

    }
}

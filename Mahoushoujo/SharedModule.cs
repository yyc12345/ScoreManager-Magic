using SMMLib.Net;
using SMMLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahoushoujo {
    public static class SharedModule {
        public static ScoreManager smmcore = new ScoreManager();
        public static LogManager logSystem;
        public static ConfigManager configManager = new ConfigManager(Information.WorkPath.Enter("mahoushoujo.cfg").Path, new Dictionary<string, string>() {
#if DEBUG
            {"Server", "http://yyc.bkt.moe:6666/" },
#else
            {"Server", "http://yyc.bkt.moe:7777/" },
#endif
            {"AssumeUTC", "1"},
            {"DisplayUTC", "1"}
        });
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMMLib.Utilities;
using ScoreManager_Magic.Core;
using SMMLib.Net;

namespace ScoreManager_Magic {
    public static class SharedModule {

        public static bool IsGameRunning = false;
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
            {"RememberedPassword", ""},
            {"CounterTopmost", "1"}
        });

        public static Anticheat anticheat = new Anticheat();
        public static NemosWatcher nemosWatcher = new NemosWatcher();
        public static WindowsRegistryHelper registryHelper = new Core.WindowsRegistryHelper();
        public static LogManager logSystem = new LogManager(Information.WorkPath.Enter("scoremanager-magic.log").Path);

        public static ScoreManager smm = new ScoreManager();

        //map hash, map name, CDK
        public static event Action<string, string, string> SelectCompetitionCallback;
        public static void Raise_SelectCompetitionCallback(string hash, string name, string cdk) {
            SelectCompetitionCallback?.Invoke(hash, name, cdk);
        }
        public static event Action<string, BsmData> NewSubmitCallback;
        public static void Raise_NewSubmitCallback(string hash, BsmData data) {
            NewSubmitCallback?.Invoke(hash, data);
        }
    }
}

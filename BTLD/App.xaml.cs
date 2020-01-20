using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BTLD {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {

            //init sharedmodule
            SharedModule.logManager = new SMMLib.Utilities.LogManager("BTLD.log", true);
            SharedModule.configManager = new SMMLib.Utilities.ConfigManager("BTLD.cfg", new Dictionary<string, string>() {
                {"PastCompetition", "[]"},
                {"NowCompetition", "[]"},
                {"Title", ""},
                {"Suffix", ""}
            });
            SharedModule.userManager = new UserAvatarManager();
            SharedModule.mapManager = new MapPreviewManager();

            base.OnStartup(e);
        }

    }
}

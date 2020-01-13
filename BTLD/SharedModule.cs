using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMMLib.Utilities;

namespace BTLD {

    public static class SharedModule {
        public static LogManager logManager;
        public static ConfigManager configManager;
        public static UserAvatarManager userManager;
        public static MapPreviewManager mapManager;
    }

    public enum MonitorPage {
        Welcome,
        Timer,
        Participant,
        MapPool,
        Grouping,
        MapPicker,
        Competition
    }
}

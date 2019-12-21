using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ScoreManager_Magic.Core {
    public class NemosWatcher {

        public NemosWatcher() {

        }

        FileSystemWatcher bsmWatcher;
        public event Action<int> LevelStart;
        public event Action<int> LevelEnd;

        private void innerWatcherProcessor(object sender, FileSystemEventArgs e) {

        }

        public void DeployNemos() {

        }

        public void RestoreNemos() {

        }

    }
}

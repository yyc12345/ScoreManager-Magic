using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScoreManager_Magic.Core {
    public class Anticheat {

        public Anticheat() {

        }

        FileSystemWatcher anticheatWatcher;
        public event Action<string> DetectCheat;

        private void innerWatcherProcessor(object sender, FileSystemEventArgs e) {

        }

        public void StartAnticheat() {

        }

        public void StopAnticheat() {

        }

    }
}

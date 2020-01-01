using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using SMMLib.Utilities;

namespace ScoreManager_Magic.Core {
    public class Anticheat {

        public Anticheat() {
            var jsonUrl = new Uri("/Resources/AnticheatCheckList.json", UriKind.Relative);
            var stm = Application.GetResourceStream(jsonUrl).Stream;
            var fs = new StreamReader(stm, Encoding.UTF8);
            checkList = JsonConvert.DeserializeObject<List<AnticheatCheckItem>>(fs.ReadToEnd());
            fs.Close();
            fs.Dispose();

            anticheatWatcher = new FileSystemWatcher();
            anticheatWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Size;
            anticheatWatcher.Filter = "*.nmo";
            anticheatWatcher.Created += innerWatcherProcessor;
            anticheatWatcher.Changed += innerWatcherProcessor;
            anticheatWatcher.Deleted += innerWatcherProcessor;
            anticheatWatcher.Renamed += innerWatcherProcessor;
            anticheatWatcher.EnableRaisingEvents = false;
        }

        List<AnticheatCheckItem> checkList;
        FileSystemWatcher anticheatWatcher;
        public event Action<string> DetectCheat;

        private void innerWatcherProcessor(object sender, FileSystemEventArgs e) {
            SharedModule.logSystem.WriteLog("[Anticheat][Info] Detected cheat: " + e.FullPath);
            DetectCheat?.Invoke(e.FullPath);
        }

        public bool StartAnticheat() {
            var gamePath = SharedModule.configManager.Configuration["BallancePath"];

            //check file
            foreach (var item in checkList) {
                switch (item.CheckMethod) {
                    case AnticheatCheckMethod.CheckExist:
                        if (!File.Exists(gamePath + item.RelativeURL)) return false;
                        if (!item.Hash.Contains(HashComput.MD5FromFile(gamePath + item.RelativeURL))) return false;
                        break;
                    case AnticheatCheckMethod.CheckNoExist:
                        if (File.Exists(gamePath + item.RelativeURL)) return false;
                        break;
                }
            }

            anticheatWatcher.Path = gamePath + "\\3D Entities";
            anticheatWatcher.EnableRaisingEvents = true;
            return true;
        }

        public void StopAnticheat() {
            anticheatWatcher.EnableRaisingEvents = false;
        }

    }

    public enum AnticheatCheckMethod {
        CheckExist,
        CheckNoExist
    }

    public class AnticheatCheckItem {
        public AnticheatCheckItem() {
            Hash = new List<string>();
        }
        public AnticheatCheckMethod CheckMethod { get; set; }
        public string RelativeURL { get; set; }
        public List<string> Hash { get; set; }
    }
}

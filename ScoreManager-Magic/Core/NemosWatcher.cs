using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using SMMLib.Utilities;

namespace ScoreManager_Magic.Core {
    public class NemosWatcher {

        public NemosWatcher() {
            bsmWatcher = new FileSystemWatcher();
            bsmWatcher.NotifyFilter = NotifyFilters.LastWrite;
            bsmWatcher.Filter = "*.bsm";
            bsmWatcher.Changed += innerWatcherProcessor;
            bsmWatcher.EnableRaisingEvents = false;
        }

        FileSystemWatcher bsmWatcher;
        int tokenCache = -2;
        public event Action<int> LevelStart;
        public event Action<BsmData> LevelEnd;
        public event Action LevelEndButFail;

        private void innerWatcherProcessor(object sender, FileSystemEventArgs e) {
            try {
                BsmRawData rawData = null;
                var gamePath = SharedModule.configManager.Configuration["GamePath"];

                //try get bsm
                if (System.IO.File.Exists(gamePath + "\\Bin\\ScoreOutput.bsm")) {
                    using (StreamReader fs = new StreamReader(gamePath + "\\Bin\\ScoreOutput.bsm")) {
                        rawData = DecodeBsm(fs.ReadToEnd());
                        fs.Close();
                    }
                    System.IO.File.Delete(gamePath + "\\Bin\\ScoreOutput.bsm");
                }

                //if get bsm, analyse it.
                if (rawData != null) {
                    if (rawData.HSScore == -1) {
                        //level start
                        LevelStart?.Invoke(rawData.LvID);
                    } else {
                        //level finish
                        //get verify status
                        if (rawData.Token != -1 && ComputHash(ComputHash(rawData.ToString()).ToString()) == rawData.HashCode && rawData.Token == tokenCache) {
                            //output
                            var res = new BsmData();
                            //change statistic data
                            int extradim = 0;
                            if (rawData.LvID == 1) extradim = 1;

                            res.InstallOn = rawData.LvID;
                            res.HSScore = rawData.HSScore - rawData.LvID * 100 - extradim;
                            res.SRTime = rawData.ReferenceTime;
                            res.Token = rawData.Token.ToString();
                            res.LifeUp = rawData.LifeUp;
                            res.LifeLost = rawData.LifeLost;
                            res.ExtraPoints = rawData.ExtraPoints;
                            res.SubExtraPoints = rawData.SubExtraPoints;
                            res.Trafo = rawData.Trafo;
                            res.Checkpoint = rawData.Checkpoint;
                            res.Verify = true;

                            //re-generate token
                            GenerateToken();

                            //return data
                            LevelEnd?.Invoke(res);

                        } else LevelEndButFail?.Invoke();
                    }
                }
            } catch (Exception ex) {
                SharedModule.logSystem.WriteLog("[NemosWatcher][Error] Get error when processing bsm: " + ex.Message + ex.StackTrace);
            }
        }

        #region inner process assistant func

        private BsmRawData DecodeBsm(string data) {
            string[] strarr = data.Split(',');
            if (strarr.Length != 12) {
                return null;
            }
            BsmRawData decoded = new BsmRawData();
            decoded.LvID = int.Parse(strarr[0]);
            decoded.HSScore = int.Parse(strarr[1]);
            decoded.SRTime = int.Parse(strarr[2]);
            decoded.Token = int.Parse(strarr[3]);
            decoded.LifeUp = int.Parse(strarr[4]);
            decoded.ExtraPoints = int.Parse(strarr[5]);
            decoded.SubExtraPoints = int.Parse(strarr[6]);
            decoded.LifeLost = int.Parse(strarr[7]);
            decoded.Trafo = int.Parse(strarr[8]);
            decoded.Checkpoint = int.Parse(strarr[9]);
            decoded.ReferenceTime = int.Parse(strarr[10]);
            decoded.HashCode = int.Parse(strarr[11]);
            return decoded;
        }

        private int ComputHash(string str) {
            int len = str.Length;
            if (len == 0) return 0;
            int res = 1;
            for (int i = 0; i < len; ++i) {
                res = (res * 257 + str[i]) % 4999999;
            }
            res = (res + res % 3 + res % 11 + res % 101) % 4999999;
            return res;
        }

        public bool GenerateToken() {
            try {
                var gamePath = SharedModule.configManager.Configuration["GamePath"];

                Random ran = new Random((int)DateTime.Now.Ticks);
                tokenCache = ran.Next(1000000);
                using (StreamWriter writer = new StreamWriter(gamePath + "\\Bin\\Token.txt", false)) { //yyc mark: default encoding: UTF-8 without BOM
                    writer.Write(tokenCache);
                    writer.Close();
                }

                return true;
            } catch (Exception) {
                tokenCache = -2;
                SharedModule.logSystem.WriteLog("[NemosWatcher][Error] Fail to create token");
                return false;
            }

        }

        #endregion

        #region public interface

        public bool StartMonitor() {
            var gamePath = SharedModule.configManager.Configuration["GamePath"];

            File.Copy(Information.WorkPath.Enter("Resources").Enter("MenuLevel.nmo").Path, gamePath + @"\3D Entities\MenuLevel.nmo", true);
            File.Copy(Information.WorkPath.Enter("Resources").Enter("ScoreManager.nmo").Path, gamePath + @"\3D Entities\ScoreManager.nmo", true);

            //init bsm watcher
            bsmWatcher.Path = gamePath + "\\Bin";
            bsmWatcher.EnableRaisingEvents = true;

            //deploy token
            if (!GenerateToken()) return false;
            return true;
        }

        public void StopMonitor() {
            bsmWatcher.EnableRaisingEvents = false;
        }

        #endregion
    }

    public class BsmData {
        public int InstallOn { get; set; }
        public int HSScore { get; set; }
        public int SRTime { get; set; }
        public string Token { get; set; }
        public int LifeUp { get; set; }
        public int ExtraPoints { get; set; }
        public int SubExtraPoints { get; set; }
        public int LifeLost { get; set; }
        public int Trafo { get; set; }
        public int Checkpoint { get; set; }
        public bool Verify { get; set; }
    }

    public class BsmRawData {
        /// <summary>
        /// 关卡ID
        /// </summary>
        public int LvID { get; set; }
        /// <summary>
        /// HS成绩
        /// </summary>
        public int HSScore { get; set; }
        /// <summary>
        /// SR时间(Ballance计时单位)
        /// </summary>
        public int SRTime { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public int Token { get; set; }
        public int LifeUp { get; set; }
        public int ExtraPoints { get; set; }
        public int SubExtraPoints { get; set; }
        public int LifeLost { get; set; }
        public int Trafo { get; set; }
        public int Checkpoint { get; set; }
        /// <summary>
        /// 行进路程
        /// </summary>
        public int TravelDistance { get; set; }
        /// <summary>
        /// SR参考时间(ms)
        /// </summary>
        public int ReferenceTime { get; set; }
        /// <summary>
        /// HashCode
        /// </summary>
        public int HashCode { get; set; }

        public override string ToString() {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                this.LvID, this.HSScore, this.SRTime, this.Token, this.LifeUp, this.ExtraPoints, this.SubExtraPoints, this.LifeLost, this.Trafo, this.Checkpoint, this.ReferenceTime);
        }
    }

}

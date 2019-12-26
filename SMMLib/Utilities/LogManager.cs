using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace SMMLib.Utilities {
    public class LogManager {

        public LogManager(string file) {
            logfs = new StreamWriter(file, false, Encoding.UTF8);
        }

        StreamWriter logfs;
        object lockfs = new object();
        public event Action<string> NewLog;

        public void WriteLog(string str) {
            Task.Factory.StartNew(() => {
                lock (lockfs) {
                    try {
                        logfs.WriteLine(str);
                    } catch {
                        return;
                    }

                    NewLog?.Invoke(str);
                }
            });
        }


    }
}

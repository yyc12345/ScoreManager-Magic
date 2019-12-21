using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMMLib.Utilities {

    public class Information {

        /// <summary>
        /// get the app's work path
        /// </summary>
        public static FilePathBuilder WorkPath {
            get {
                return new FilePathBuilder(Environment.CurrentDirectory);
            }
        }

        public static string GetSMMVersion() { return "0.5"; }
        public static string GetSMMProtocolVersion() { return "1"; }

    }

}

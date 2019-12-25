using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMMLib.Utilities {
    public static class HashComput {
        public static string SHA256FromString(string str) {
            System.Security.Cryptography.SHA256 sha256 = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] retVal = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));

            return GetByteArray(retVal);
        }

        public static string SHA256FromFile(string file) {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            System.Security.Cryptography.SHA256 sha256 = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] retVal = sha256.ComputeHash(fs);
            fs.Close();

            return GetByteArray(retVal);
        }

        public static string MD5FromFile(string file) {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            return GetByteArray(retVal);
        }

        private static string GetByteArray(byte[] arr) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++) {
                sb.Append(arr[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

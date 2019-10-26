using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMM_Frontend.Utilities {

    public class StandardResponse {
        public StandardResponse(bool a, string b) {
            IsSuccess = a;
            Description = b;
        }
        public bool IsSuccess = true;
        public string Description = "";
    }

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

        private static string GetByteArray(byte[] arr) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++) {
                sb.Append(arr[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }


}

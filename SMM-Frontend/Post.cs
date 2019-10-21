using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SMM_Frontend {

    internal static class InternetMethod {

        internal static (bool success, string result) Post(string url, Dictionary<string, string> parameter) {
            try {
                var (res, stream) = LongPost(url, parameter);
                return (res, stream.ReadToEnd());
            } catch (Exception) {
                return (false, "");
            }
        }

        internal static (bool success, StreamReader stream) LongPost(string url, Dictionary<string, string> parameter) {
            try {
                var request = (HttpWebRequest)WebRequest.Create(url);

                var real_param = "";
                if (parameter.Count == 0) goto real_post;
                var cache = new List<string>();
                foreach (var item in parameter.Keys) {
                    cache.Add($"{item}={System.Web.HttpUtility.UrlEncode(parameter[item], Encoding.UTF8)}");
                }
                real_param = String.Join("&", cache);

                real_post:
                var data = Encoding.UTF8.GetBytes(real_param);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK) return (false, null);
                var responseStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                return (true, responseStream);
            } catch (Exception) {
                return (false, null);
            }
        }

    }

}

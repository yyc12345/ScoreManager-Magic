using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SMMLib.Net {

    internal static class NetworkMethod {

        internal static string Post(string url, Dictionary<string, string> parameter) {
            var stream = LongPost(url, parameter);
            return stream.ReadToEnd();
        }

        internal static StreamReader LongPost(string url, Dictionary<string, string> parameter) {
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
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream()) {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("Remote services return a error.");
            var responseStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

            return responseStream;
        }

    }

}

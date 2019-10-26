using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMM_Frontend.Structure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMM_Frontend.Utilities;

namespace SMM_Frontend {
    public static class CoreMethod {

        public static void ChangeDomain(string str) => CoreUrl.CoreURL = str;

        private static string JsonDecoder(string str) {
            JObject decodeData = (JObject)JsonConvert.DeserializeObject(str);
            if (decodeData["code"].ToString() != "200") throw new Exception(decodeData["err"].ToString());
            return decodeData["data"].ToString();
        }

        public static (StandardResponse status, string token, int priority) Login(string user, string password) {
            try {
                //get salt
                var data = InternetMethod.Post(CoreUrl.Salt, new Dictionary<string, string>() {
                    {"name", user}
                });
                var realData = JsonDecoder(data);
                var decodeData = JsonConvert.DeserializeObject<Protocol_Salt>(realData);
                var rnd = decodeData.rnd;

                //calc number
                var authStr = HashComput.SHA256FromString(HashComput.SHA256FromString(password) + rnd.ToString());

                //login
                data = InternetMethod.Post(CoreUrl.Login, new Dictionary<string, string>() {
                    {"name", user },
                    {"hash", authStr }
                });
                realData = JsonDecoder(data);
                var decodeData2 = JsonConvert.DeserializeObject<Protocol_Login>(realData);
                return (new StandardResponse(true, ""), decodeData2.token, decodeData2.priority);
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), "", 0);
            }
        }

        public static StandardResponse Logout(string token) {
            try {
                var data = InternetMethod.Post(CoreUrl.Logout, new Dictionary<string, string>() {
                    {"token", token}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public static StandardResponse Submit(string token,
            int installOn,
            string map,
            int score,
            int srTime,
            int lifeUp,
            int lifeLost,
            int extraPoint,
            int subExtraPoint,
            int trafo,
            int checkpoint,
            bool verify,
            string bsmToken) {

            try {
                var localTimestamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                var data = InternetMethod.Post(CoreUrl.Logout, new Dictionary<string, string>() {
                    {"token", token},
                    {"installOn", installOn.ToString()},
                    {"map",map },
                    {"score",score.ToString() },
                    {"srTime",srTime.ToString() },
                    {"lifeUp",lifeUp.ToString() },
                    {"lifeLost",lifeLost.ToString() },
                    {"extraPoint",extraPoint.ToString() },
                    {"subExtraPoint",subExtraPoint.ToString() },
                    {"trafo",trafo.ToString() },
                    {"checkpoint",checkpoint.ToString() },
                    {"verify",Convert.ToInt32(verify).ToString() },
                    {"bsmToken",bsmToken },
                    {"localTime",localTimestamp.ToString() }
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMMLib.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMMLib.Utilities;

namespace SMMLib.Net {
    public static class SMMCoreMethod {

        public static void ChangeDomain(string str) => SMMCoreUrl.RequestBaseUrl = str;

        private static string JsonDecoder(string str) {
            JObject decodeData = (JObject)JsonConvert.DeserializeObject(str);
            if (decodeData["code"].ToString() != "200") throw new Exception(decodeData["err"].ToString());
            return decodeData["data"].ToString();
        }

        public static (StandardResponse status, string version) Version() {
            try {
                var data = NetworkMethod.Post(SMMCoreUrl.Version, new Dictionary<string, string>());
                var realData = JsonDecoder(data);
                var decodeData = JsonConvert.DeserializeObject<Structure_Version>(realData);

                return (new StandardResponse(true, ""), decodeData.ver);
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), "");
            }
        }
             
        public static (StandardResponse status, string token, int priority) Login(string user, string password) {
            try {
                //========================================get salt
                var data = NetworkMethod.Post(SMMCoreUrl.Salt, new Dictionary<string, string>() {
                    {"name", user}
                });
                var realData = JsonDecoder(data);
                var decodeData = JsonConvert.DeserializeObject<Structure_Salt>(realData);
                var rnd = decodeData.rnd;

                //calc number
                var authStr = HashComput.SHA256FromString(HashComput.SHA256FromString(password) + rnd.ToString());

                //========================================login
                data = NetworkMethod.Post(SMMCoreUrl.Login, new Dictionary<string, string>() {
                    {"name", user },
                    {"hash", authStr }
                });
                realData = JsonDecoder(data);
                var decodeData2 = JsonConvert.DeserializeObject<Structure_Login>(realData);
                return (new StandardResponse(true, ""), decodeData2.token, decodeData2.priority);
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), "", 0);
            }
        }

        public static StandardResponse Logout(string token) {
            try {
                var data = NetworkMethod.Post(SMMCoreUrl.Logout, new Dictionary<string, string>() {
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
                var data = NetworkMethod.Post(SMMCoreUrl.Submit, new Dictionary<string, string>() {
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

        public static StandardResponse ChangePassword(string token, string newPassword) {
            try {
                //comput password
                var realPassword = HashComput.SHA256FromString(newPassword);

                //post request
                var data = NetworkMethod.Post(SMMCoreUrl.ChangePassword, new Dictionary<string, string>() {
                    {"token", token},
                    {"newPassword", realPassword}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public static (StandardResponse status, List<Structure_GetFutureCompetition> data) GetFutureCompetition(string token) {
            try {
                var data = NetworkMethod.Post(SMMCoreUrl.GetFutureCompetition, new Dictionary<string, string>() {
                    {"token", token}
                });
                //check result
                var realData = JsonDecoder(data);
                //decode data and return
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<Structure_GetFutureCompetition>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public static (StandardResponse status, List<Structure_GetMapName> data) GetMapName(string token, List<string> mapHashs) {
            try {
                var data = NetworkMethod.Post(SMMCoreUrl.GetMapName, new Dictionary<string, string>() {
                    {"token", token},
                    {"mapHash", JsonConvert.SerializeObject(mapHashs)}
                });
                //check result
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<Structure_GetMapName>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public static (StandardResponse status, List<Structure_GetTournament> data) GetTournament(string token) {
            try {
                var data = NetworkMethod.Post(SMMCoreUrl.GetTournament, new Dictionary<string, string>() {
                    {"token", token}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<Structure_GetTournament>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public static StandardResponse RegisterTournament(string token, string tournament) {
            try {
                var data = NetworkMethod.Post(SMMCoreUrl.Logout, new Dictionary<string, string>() {
                    {"token", token},
                    {"tournament", tournament}
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

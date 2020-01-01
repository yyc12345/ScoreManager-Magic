using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMMLib.Data.SMMStructure;
using SMMLib.Data.SMMInputBuilder;
using SMMLib.Data;
using SMMLib.Utilities;
using System.Threading.Tasks;

namespace SMMLib.Net {
    public class ScoreManager {

        public ScoreManager() {
            Token = "";
            Priority = SM_Priority.None;
        }

        private string _token;
        public string Token {
            get {
                if (_token == "") throw new Exception("No token.");
                else return _token;
            }
            private set {
                _token = value;
            }
        }
        public SM_Priority Priority { get; private set; }
        public string Username { get; private set; }
        public string ServerDomain {
            get {
                return CoreUrl.RequestBaseUrl;
            }
            set {
                CoreUrl.RequestBaseUrl = value;
            }
        }

        #region utility methods

        public void ChangeDomain(string str) => CoreUrl.RequestBaseUrl = str;

        private string JsonDecoder(string str) {
            JObject decodeData = (JObject)JsonConvert.DeserializeObject(str);
            if (decodeData["code"].ToString() != "200") throw new Exception(decodeData["err"].ToString());
            return decodeData["data"].ToString();
        }

        #endregion

        #region user methods

        public (StandardResponse status, string version) Version() {
            try {
                var data = NetworkMethod.Post(CoreUrl.Version, new Dictionary<string, string>());
                var realData = JsonDecoder(data);
                var decodeData = JsonConvert.DeserializeObject<Data.SMMStructure.Version>(realData);

                return (new StandardResponse(true, ""), decodeData.ver);
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), "");
            }
        }

        public StandardResponse Login(string user, string password) {
            try {
                //========================================get salt
                var data = NetworkMethod.Post(CoreUrl.Salt, new Dictionary<string, string>() {
                    {"name", user}
                });
                var realData = JsonDecoder(data);
                var decodeData = JsonConvert.DeserializeObject<Salt>(realData);
                var rnd = decodeData.rnd;

                //calc number
                var authStr = HashComput.SHA256FromString(HashComput.SHA256FromString(password) + rnd.ToString());

                //========================================login
                data = NetworkMethod.Post(CoreUrl.Login, new Dictionary<string, string>() {
                    {"name", user },
                    {"hash", authStr }
                });
                realData = JsonDecoder(data);
                var decodeData2 = JsonConvert.DeserializeObject<Login>(realData);

                //set internal value
                Token = decodeData2.token;
                Priority = (SM_Priority)decodeData2.priority;
                Username = user;
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse Logout() {
            try {
                var data = NetworkMethod.Post(CoreUrl.Logout, new Dictionary<string, string>() {
                    {"token", Token}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);

                //reset token
                Token = "";
                Priority = SM_Priority.None;
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse Submit(int installOn,
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
                var data = NetworkMethod.Post(CoreUrl.Submit, new Dictionary<string, string>() {
                    {"token", Token},
                    {"installedOn", installOn.ToString()},
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

        public StandardResponse ChangePassword(string newPassword) {
            try {
                //comput password
                var realPassword = HashComput.SHA256FromString(newPassword);

                //post request
                var data = NetworkMethod.Post(CoreUrl.ChangePassword, new Dictionary<string, string>() {
                    {"token", Token},
                    {"newPassword", realPassword}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public (StandardResponse status, List<GetFutureCompetition> data) GetFutureCompetition() {
            try {
                var data = NetworkMethod.Post(CoreUrl.GetFutureCompetition, new Dictionary<string, string>() {
                    {"token", Token}
                });
                //check result
                var realData = JsonDecoder(data);
                //decode data and return
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<GetFutureCompetition>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public (StandardResponse status, List<GetMapName> data) GetMapName(List<string> mapHashs) {
            try {
                var data = NetworkMethod.Post(CoreUrl.GetMapName, new Dictionary<string, string>() {
                    {"token", Token},
                    {"mapHash", JsonConvert.SerializeObject(mapHashs)}
                });
                //check result
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<GetMapName>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public (StandardResponse status, List<GetTournament> data) GetTournament() {
            try {
                var data = NetworkMethod.Post(CoreUrl.GetTournament, new Dictionary<string, string>() {
                    {"token", Token}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<GetTournament>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public StandardResponse RegisterTournament(string tournament) {
            try {
                var data = NetworkMethod.Post(CoreUrl.RegisterTournament, new Dictionary<string, string>() {
                    {"token", Token},
                    {"tournament", tournament}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        #endregion

        #region admin methods

        public (StandardResponse status, List<OperationUserQuery> data) OperationUser_Query(UserQueryFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationUser, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "query"},
                    {"filterRules", JsonConvert.SerializeObject(filter)}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<OperationUserQuery>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public StandardResponse OperationUser_Add(UserAddBuilder inputValues) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationUser, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "add"},
                    {"newValues", JsonConvert.SerializeObject(inputValues)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse OperationUser_Delete(List<string> target) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationUser, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "delete"},
                    {"target", JsonConvert.SerializeObject(target)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse OperationUser_Update(List<string> target, UserUpdateFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationUser, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "update"},
                    {"target", JsonConvert.SerializeObject(target)},
                    {"newValues", JsonConvert.SerializeObject(filter)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public (StandardResponse status, List<OperationCompetitionQuery> data) OperationCompetition_Query(CompetitionQueryFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationCompetition, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "query"},
                    {"filterRules", JsonConvert.SerializeObject(filter)}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<OperationCompetitionQuery>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public (StandardResponse status, long inserID) OperationCompetition_Add(CompetitionAddBuilder inputValues) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationCompetition, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "add"},
                    {"newValues", JsonConvert.SerializeObject(inputValues)}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), long.Parse(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), -1);
            }
        }

        public StandardResponse OperationCompetition_Delete(List<long> target) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationCompetition, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "delete"},
                    {"target", JsonConvert.SerializeObject(target)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse OperationCompetition_Update(long target, CompetitionUpdateFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationCompetition, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "update"},
                    {"target", target.ToString()},
                    {"newValues", JsonConvert.SerializeObject(filter)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public (StandardResponse status, List<OperationRecordQuery>) OperationRecord_Query(RecordQueryFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationRecord, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "query"},
                    {"filterRules", JsonConvert.SerializeObject(filter)}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<OperationRecordQuery>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public (StandardResponse status, List<OperationTournamentQuery> data) OperationTournament_Query(TournamentQueryFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationTournament, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "query"},
                    {"filterRules", JsonConvert.SerializeObject(filter)}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<OperationTournamentQuery>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public StandardResponse OperationTournament_Add(TournamentAddBuilder inputValues) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationTournament, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "add"},
                    {"newValues", JsonConvert.SerializeObject(inputValues)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse OperationTournament_Delete(string target) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationTournament, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "delete"},
                    {"target", target}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse OperationTournament_Update(string target, TournamentUpdateFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationTournament, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "update"},
                    {"target", target},
                    {"newValues", JsonConvert.SerializeObject(filter)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public (StandardResponse status, List<OperationRegistryQuery> data) OperationRegistry_Query(RegistryQueryFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationRegistry, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "query"},
                    {"filterRules", JsonConvert.SerializeObject(filter)}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<OperationRegistryQuery>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public StandardResponse OperationRegistry_Add(RegistryAddDeleteBuilder inputValues) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationRegistry, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "add"},
                    {"newValues", JsonConvert.SerializeObject(inputValues)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse OperationRegistry_Delete(RegistryAddDeleteBuilder inputValues) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationRegistry, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "delete"},
                    {"target", JsonConvert.SerializeObject(inputValues)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public (StandardResponse status, List<OperationMapPoolQuery> data) OperationMapPool_Query(MapPoolQueryFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationMapPool, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "query"},
                    {"filterRules", JsonConvert.SerializeObject(filter)}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<OperationMapPoolQuery>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public StandardResponse OperationMapPool_Add(MapPoolAddDeleteBuilder inputValues) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationMapPool, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "add"},
                    {"newValues", JsonConvert.SerializeObject(inputValues)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse OperationMapPool_Delete(MapPoolAddDeleteBuilder inputValues) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationMapPool, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "delete"},
                    {"target", JsonConvert.SerializeObject(inputValues)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public (StandardResponse status, List<OperationMapQuery> data) OperationMap_Query(MapQueryFilter filter) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationMap, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "query"},
                    {"filterRules", JsonConvert.SerializeObject(filter)}
                });
                //check result and return data
                var realData = JsonDecoder(data);
                return (new StandardResponse(true, ""), JsonConvert.DeserializeObject<List<OperationMapQuery>>(realData));
            } catch (Exception e) {
                return (new StandardResponse(false, e.Message), null);
            }
        }

        public StandardResponse OperationMap_Add(MapAddBuilder inputValues) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationMap, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "add"},
                    {"newValues", JsonConvert.SerializeObject(inputValues)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        public StandardResponse OperationMap_Delete(List<string> target) {
            try {
                var data = NetworkMethod.Post(CoreUrl.OperationMap, new Dictionary<string, string>() {
                    {"token", Token},
                    {"method", "delete"},
                    {"target", JsonConvert.SerializeObject(target)}
                });
                //invoke decoder to ensure check result
                JsonDecoder(data);
                return new StandardResponse(true, "");
            } catch (Exception e) {
                return new StandardResponse(false, e.Message);
            }
        }

        #endregion

    }
}

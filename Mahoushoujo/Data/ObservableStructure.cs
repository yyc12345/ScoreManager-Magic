using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMMLib.Utilities;
using SMMLib.Data;

namespace Mahoushoujo.Data {

    public class OperationUserQuery : SMMLib.Data.SMMStructure.OperationUserQuery {
        public OperationUserQuery(SMMLib.Data.SMMStructure.OperationUserQuery root) {
            this.sm_name = root.sm_name;
            this.sm_password = root.sm_password;
            this.sm_registration = root.sm_registration;
            this.sm_priority = root.sm_priority;
            this.sm_salt = root.sm_salt;
            this.sm_token = root.sm_token;
            this.sm_expireOn = root.sm_expireOn;
        }

        public string conv_registration {
            get {
                if (SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean()) return sm_registration.ConvertToDatetime().ToString();
                else return sm_registration.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_priority {
            get {
                List<string> res = new List<string>();
                if ((sm_priority & SM_Priority.User) == SM_Priority.User) res.Add(SM_Priority.User.ToString());
                if ((sm_priority & SM_Priority.Live) == SM_Priority.Live) res.Add(SM_Priority.Live.ToString());
                if ((sm_priority & SM_Priority.Speedrun) == SM_Priority.Speedrun) res.Add(SM_Priority.Speedrun.ToString());
                if ((sm_priority & SM_Priority.Admin) == SM_Priority.Admin) res.Add(SM_Priority.Admin.ToString());

                if (res.Count == 0) return "None";
                else return string.Join(", ", res);
            }
            set {; }
        }

        public string conv_expireOn {
            get {
                if (SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean()) return sm_expireOn.ConvertToDatetime().ToString();
                else return sm_expireOn.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }
    }

    public class OperationCompetitionQuery : SMMLib.Data.SMMStructure.OperationCompetitionQuery {
        public OperationCompetitionQuery(SMMLib.Data.SMMStructure.OperationCompetitionQuery root) {
            this.sm_id = root.sm_id;
            this.sm_result = root.sm_result;
            this.sm_startDate = root.sm_startDate;
            this.sm_endDate = root.sm_endDate;
            this.sm_judgeEndDate = root.sm_judgeEndDate;
            this.sm_map = root.sm_map;
            this.sm_banMap = root.sm_banMap;
            this.sm_cdk = root.sm_cdk;
            this.sm_winner = root.sm_winner;
            this.sm_participant = root.sm_participant;
        }

        public string conv_startDate {
            get {
                if (SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean()) return sm_startDate.ConvertToDatetime().ToString();
                else return sm_startDate.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_endDate {
            get {
                if (SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean()) return sm_endDate.ConvertToDatetime().ToString();
                else return sm_endDate.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_judgeEndDate {
            get {
                if (SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean()) return sm_judgeEndDate.ConvertToDatetime().ToString();
                else return sm_judgeEndDate.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_map {
            get {
                return sm_map;
            }
            set {; }
        }

        public string conv_banMap {
            get {
                return sm_banMap;
            }
            set {; }
        }

        public string conv_participant {
            get {
                return string.Join(", ", sm_participant);
            }
            set {; }
        }
    }

    public class OperationRecordQuery : SMMLib.Data.SMMStructure.OperationRecordQuery {
        public OperationRecordQuery(SMMLib.Data.SMMStructure.OperationRecordQuery root) {
            this.sm_name = root.sm_name;
            this.sm_installedOn = root.sm_installedOn;
            this.sm_map = root.sm_map;
            this.sm_score = root.sm_score;
            this.sm_srTime = root.sm_srTime;
            this.sm_lifeUp = root.sm_lifeUp;
            this.sm_lifeLost = root.sm_lifeLost;
            this.sm_extraPoint = root.sm_extraPoint;
            this.sm_subExtraPoint = root.sm_subExtraPoint;
            this.sm_trafo = root.sm_trafo;
            this.sm_checkpoint = root.sm_checkpoint;
            this.sm_verify = root.sm_verify;
            this.sm_token = root.sm_token;
            this.sm_localUTC = root.sm_localUTC;
            this.sm_serverUTC = root.sm_serverUTC;
        }

        public string conv_map {
            get {
                return sm_map;
            }
            set {; }
        }

        public string conv_srTime {
            get {
                return sm_srTime.SRTimeFormat();
            }
            set {; }
        }

        public string conv_verify {
            get {
                return sm_verify.ConvertToBoolean().ToString();
            }
            set {; }
        }

        public string conv_localUTC {
            get {
                if (SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean()) return sm_localUTC.ConvertToDatetime().ToString();
                else return sm_localUTC.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_serverUTC {
            get {
                if (SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean()) return sm_serverUTC.ConvertToDatetime().ToString();
                else return sm_serverUTC.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }


    }

    public class OperationTournamentQuery : SMMLib.Data.SMMStructure.OperationTournamentQuery {
        public OperationTournamentQuery(SMMLib.Data.SMMStructure.OperationTournamentQuery root) {
            this.sm_tournament = root.sm_tournament;
            this.sm_startDate = root.sm_startDate;
            this.sm_endDate = root.sm_endDate;
            this.sm_schedule = root.sm_schedule;
        }

        public string conv_startDate {
            get {
                if (SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean()) return sm_startDate.ConvertToDatetime().ToString();
                else return sm_startDate.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_endDate {
            get {
                if (SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean()) return sm_endDate.ConvertToDatetime().ToString();
                else return sm_endDate.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_schedule {
            get {
                if (sm_schedule.Length < 20) return sm_schedule;
                else return sm_schedule.Substring(0, 20) + " ...";
            }
            set {; }
        }

    }

    public class OperationRegistryQuery : SMMLib.Data.SMMStructure.OperationRegistryQuery {
        public OperationRegistryQuery(SMMLib.Data.SMMStructure.OperationRegistryQuery root) {
            this.sm_user = root.sm_user;
            this.sm_tournament = root.sm_tournament;
        }
    }

    public class OperationMapPoolQuery : SMMLib.Data.SMMStructure.OperationMapPoolQuery {
        public OperationMapPoolQuery(SMMLib.Data.SMMStructure.OperationMapPoolQuery root) {
            this.sm_hash = root.sm_hash;
            this.sm_tournament = root.sm_tournament;
        }

        public string conv_hash {
            get {
                return sm_hash;
            }
            set {; }
        }

    }

    public class OperationMapQuery : SMMLib.Data.SMMStructure.OperationMapQuery {
        public OperationMapQuery(SMMLib.Data.SMMStructure.OperationMapQuery root) {
            this.sm_name = root.sm_name;
            this.sm_i18n = root.sm_i18n;
            this.sm_hash = root.sm_hash;

        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMMLib.Data.SMMStructure {

    #region user structure

    public class Salt {
        public int rnd { get; set; }
    }

    public class Login {
        public string token { get; set; }
        public int priority { get; set; }
    }

    public class Version {
        public string ver { get; set; }
    }

    public class GetFutureCompetition {
        public int sm_id { get; set; }
        public long sm_startDate { get; set; }
        public long sm_endDate { get; set; }
        public string sm_map { get; set; }
        public string sm_cdk { get; set; }
        public List<string> sm_participant { get; set; }
    }

    public class GetMapName {
        public string sm_name { get; set; }
        public string sm_i18n { get; set; }
        public string sm_hash { get; set; }
    }

    public class GetTournament {
        public string sm_tournament { get; set; }
        public long sm_startDate { get; set; }
        public long sm_endDate { get; set; }
        public bool participated { get; set; }
    }

    #endregion

    #region admin structure

    public class OperationUserQuery {
        public string sm_name { get; set; }
        public string sm_password { get; set; }
        public long sm_registration { get; set; }
        public SM_Priority sm_priority { get; set; }
        public int sm_salt { get; set; }
        public string sm_token { get; set; }
        public long sm_expireOn { get; set; }
    }

    public class OperationCompetitionQuery {
        public long sm_id { get; set; }
        public string sm_result { get; set; } //can be serilized as List<OperationCompetitionQuery_SMResult>
        public long sm_startDate { get; set; }
        public long sm_endDate { get; set; }
        public long sm_judgeEndDate { get; set; }
        public string sm_map { get; set; }
        public string sm_banMap { get; set; } // can be serilized as List<string>
        public string sm_cdk { get; set; }
        public string sm_winner { get; set; }
        public List<string> sm_participant { get; set; }
    }

    //this class should be used in client. not in lib
    //public class OperationCompetitionQuery_SMResult {
    //    public string name { get; set; }
    //    public int result { get; set; }
    //    public string link { get; set; }
    //}

    public class OperationRecordQuery {
        public string sm_name { get; set; }
        public int sm_installedOn { get; set; }
        public string sm_map { get; set; }
        public int sm_score { get; set; }
        public int sm_srTime { get; set; }
        public int sm_lifeUp { get; set; }
        public int sm_lifeLost { get; set; }
        public int sm_extraPoint { get; set; }
        public int sm_subExtraPoint { get; set; }
        public int sm_trafo { get; set; }
        public int sm_checkpoint { get; set; }
        public int sm_verify { get; set; }
        public string sm_token { get; set; }
        public long sm_localUTC { get; set; }
        public long sm_serverUTC { get; set; }
    }

    public class OperationTournamentQuery {
        public string sm_tournament { get; set; }
        public long sm_startDate { get; set; }
        public long sm_endDate { get; set; }
        public string sm_schedule { get; set; }
    }

    public class OperationRegistryQuery {
        public string sm_user { get; set; }
        public string sm_tournament { get; set; }
    }

    public class OperationMapPoolQuery {
        public string sm_hash { get; set; }
        public string sm_tournament { get; set; }
    }

    public class OperationMapQuery {
        public string sm_name { get; set; }
        public string sm_i18n { get; set; }
        public string sm_hash { get; set; }
    }

    #endregion

}

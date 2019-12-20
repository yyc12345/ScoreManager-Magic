using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMMLib.Data.SMMStructure {

    [Flags]
    public enum SM_Priority {
        None = 0,
        User = 0b1,
        Live = 0b10,
        Speedrun = 0b100,
        Admin = 0b1000
    }

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
        public bool participanted { get; set; }
    }

    #endregion

    #region admin structure

    public class OperationUserQuery {
        public string sm_name { get; set; }
        public string sm_password { get; set; }
        public long sm_registration { get; set; }
        public int sm_priority { get; set; }
        public int sm_salt { get; set; }
        public string sm_token { get; set; }
        public long sm_expireOn { get; set; }
    }

    public class OperationCompetitionQuery {
        public long sm_id { get; set; }
        public List<OperationCompetitionQuery_SMResult> sm_result { get; set; }
        public long sm_startDate { get; set; }
        public long sm_endDate { get; set; }
        public long sm_judgeEndDate { get; set; }
        public string sm_map { get; set; }
        public List<string> sm_banMap { get; set; }
        public string sm_cdk { get; set; }
        public string winner { get; set; }
        public List<string> sm_participant { get; set; }
    }

    public class OperationCompetitionQuery_SMResult {
        public string name { get; set; }
        public int result { get; set; }
        public string link { get; set; }
    }

    public class OperationRecordQuery {
        public string sm_name { get; set; }
        public int installedOn { get; set; }
        public string sm_map { get; set; }
        public int sm_score { get; set; }
        public int sm_srTime { get; set; }
        public int sm_lifeUp { get; set; }
        public int sm_lifeLost { get; set; }
        public int sm_extraPoint { get; set; }
        public int sm_subExtraPoint { get; set; }
        public int sm_trafo { get; set; }
        public int sm_checkpoint { get; set; }
        public uint sm_verify { get; set; }
        public string sm_token { get; set; }
        public ulong sm_localUTC { get; set; }
        public ulong sm_serverUTC { get; set; }
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

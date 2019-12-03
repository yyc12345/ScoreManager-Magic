using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMMLib.Data {

    [Flags]
    public enum SM_Priority {
        None = 0,
        User = 0b1,
        Live = 0b10,
        Speedrun = 0b100,
        Admin = 0b1000
    }

    public class Structure_Salt {
        public int rnd { get; set; }
    }

    public class Structure_Login {
        public string token { get; set; }
        public int priority { get; set; }
    }

    public class Structure_Version {
        public string ver { get; set; }
    }

    public class Structure_GetFutureCompetition {
        public int sm_id { get; set; }
        public long sm_startDate { get; set; }
        public long sm_endDate { get; set; }
        public string sm_map { get; set; }
        public string sm_cdk { get; set; }
        public List<string> sm_participant { get; set; }
    }

    public class Structure_GetMapName {
        public string sm_name { get; set; }
        public string sm_i18n { get; set; }
        public string sm_hash { get; set; }
    }

    public class Structure_GetTournament {
        public string sm_tournament { get; set; }
        public long sm_startDate { get; set; }
        public long sm_endDate { get; set; }
        public bool participanted { get; set; }
    }

}

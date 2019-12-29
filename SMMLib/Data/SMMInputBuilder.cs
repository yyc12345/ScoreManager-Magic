using SMMLib.Data.SMMStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMMLib.Utilities;

namespace SMMLib.Data.SMMInputBuilder {

    //filter: have ShouldSerializ... method. optional property convert
    //Builder: don't have ShouldSerializ... method. all property will be converted.

    #region filter (query update)

    public class UserQueryFilter {
        public UserQueryFilter(bool useName, string name = default) {
            this.useName = useName;
            this.name = name;
        }

        private bool useName { get; set; }
        public string name { get; set; }

        public bool ShouldSerializename() => useName;
    }

    public class UserUpdateFilter {
        public UserUpdateFilter(bool usePassword, bool usePriority, bool useExpireOn,
            string password = default, SM_Priority priority = default, long expireOn = default) {
            this.usePassword = usePassword;
            this.usePriority = usePriority;
            this.useExpireOn = useExpireOn;
            this.password = usePassword ? HashComput.SHA256FromString(password) : "";
            this.priority = priority;
            this.expireOn = expireOn;
        }

        private bool usePassword { get; set; }
        private bool usePriority { get; set; }
        private bool useExpireOn { get; set; }
        public string password { get; set; }
        public SM_Priority priority { get; set; }
        public long expireOn { get; set; }


        public bool ShouldSerializepassword() => usePassword;
        public bool ShouldSerializepriority() => usePriority;
        public bool ShouldSerializeexpireOn() => useExpireOn;
    }

    public class CompetitionQueryFilter {
        public CompetitionQueryFilter(bool useId, bool useName, bool useStartDate, bool useEndDate, bool useCDK, bool useMap,
            List<long> id = default, string name = default, long startDate = default, long endDate = default, string cdk = default, string map = default) {
            this.id = id;
            this.name = name;
            this.startDate = startDate;
            this.endDate = endDate;
            this.cdk = cdk;
            this.map = map;
            this.useId = useId;
            this.useName = useName;
            this.useStartDate = useStartDate;
            this.useEndDate = useEndDate;
            this.useCDK = useCDK;
            this.useMap = useMap;
        }

        public List<long> id { get; set; }
        public string name { get; set; }
        public long startDate { get; set; }
        public long endDate { get; set; }
        public string cdk { get; set; }
        public string map { get; set; }
        private bool useId { get; set; }
        private bool useName { get; set; }
        private bool useStartDate { get; set; }
        private bool useEndDate { get; set; }
        private bool useCDK { get; set; }
        private bool useMap { get; set; }

        public bool ShouldSerializeid() => useId;
        public bool ShouldSerializename() => useName;
        public bool ShouldSerializestartDate() => useStartDate;
        public bool ShouldSerializeendDate() => useEndDate;
        public bool ShouldSerializecdk() => useCDK;
        public bool ShouldSerializemap() => useMap;
    }

    public class CompetitionUpdateFilter {
        public CompetitionUpdateFilter(bool useResult, bool useMap, bool useBanMap, bool useWinner,
            string result = default, string map = default, string banMap = default, string winner = default) {
            this.result = result;
            this.map = map;
            this.banMap = banMap;
            this.winner = winner;
            this.useResult = useResult;
            this.useMap = useMap;
            this.useBanMap = useBanMap;
            this.useWinner = useWinner;
        }

        public string result { get; set; }
        public string map { get; set; }
        public string banMap { get; set; }
        public string winner { get; set; }
        private bool useResult { get; set; }
        private bool useMap { get; set; }
        private bool useBanMap { get; set; }
        private bool useWinner { get; set; }

        public bool ShouldSerializeresult() => useResult;
        public bool ShouldSerializemap() => useMap;
        public bool ShouldSerializebanMap() => useBanMap;
        public bool ShouldSerializewinner() => useWinner;

    }

    public class RecordQueryFilter {
        public RecordQueryFilter(bool useInstallOn, bool useName, bool useStartDate, bool useEndDate, bool useScore, bool useTime, bool useMap,
            int installOn = default, string name = default, long startDate = default, long endDate = default, int score = default, int time = default, string map = default) {
            this.installOn = installOn;
            this.name = name;
            this.startDate = startDate;
            this.endDate = endDate;
            this.score = score;
            this.time = time;
            this.map = map;
            this.useInstallOn = useInstallOn;
            this.useName = useName;
            this.useStartDate = useStartDate;
            this.useEndDate = useEndDate;
            this.useScore = useScore;
            this.useTime = useTime;
            this.useMap = useMap;
        }

        public int installOn { get; set; }
        public string name { get; set; }
        public long startDate { get; set; }
        public long endDate { get; set; }
        public int score { get; set; }
        public int time { get; set; }
        public string map { get; set; }
        private bool useInstallOn { get; set; }
        private bool useName { get; set; }
        private bool useStartDate { get; set; }
        private bool useEndDate { get; set; }
        private bool useScore { get; set; }
        private bool useTime { get; set; }
        private bool useMap { get; set; }

        public bool ShouldSerializeinstallOn() => useInstallOn;
        public bool ShouldSerializename() => useName;
        public bool ShouldSerializestartDate() => useStartDate;
        public bool ShouldSerializeendDate() => useEndDate;
        public bool ShouldSerializescore() => useScore;
        public bool ShouldSerializetime() => useTime;
        public bool ShouldSerializemap() => useMap;

    }

    public class TournamentQueryFilter {
        public TournamentQueryFilter(bool useName, string name = default) {
            this.useName = useName;
            this.name = name;
        }

        private bool useName { get; set; }
        public string name { get; set; }

        public bool ShouldSerializename() => useName;
    }

    public class TournamentUpdateFilter {
        public TournamentUpdateFilter(bool useStartDate, bool useEndDate, bool useSchedule,
        long startDate = default, long endDate = default, string schedule = default) {
            this.startDate = startDate;
            this.endDate = endDate;
            this.schedule = schedule;
            this.useStartDate = useStartDate;
            this.useEndDate = useEndDate;
            this.useSchedule = useSchedule;
        }

        public long startDate { get; set; }
        public long endDate { get; set; }
        public string schedule { get; set; }
        private bool useStartDate { get; set; }
        private bool useEndDate { get; set; }
        private bool useSchedule { get; set; }

        public bool ShouldSerializestartDate() => useStartDate;
        public bool ShouldSerializeendDate() => useEndDate;
        public bool ShouldSerializeschedule() => useSchedule;

    }

    public class RegistryQueryFilter {
        public RegistryQueryFilter(bool useName, bool useTournament,
            string name = default, string tournament = default) {
            this.name = name;
            this.tournament = tournament;
            this.useName = useName;
            this.useTournament = useTournament;
        }

        public string name { get; set; }
        public string tournament { get; set; }
        private bool useName { get; set; }
        private bool useTournament { get; set; }

        public bool ShouldSerializename() => useName;
        public bool ShouldSerializetournament() => useTournament;

    }

    public class MapPoolQueryFilter {
        public MapPoolQueryFilter(bool useHash, bool useTournament,
        string hash = default, string tournament = default) {
            this.hash = hash;
            this.tournament = tournament;
            this.useHash = useHash;
            this.useTournament = useTournament;
        }

        public string hash { get; set; }
        public string tournament { get; set; }
        private bool useHash { get; set; }
        private bool useTournament { get; set; }

        public bool ShouldSerializehash() => useHash;
        public bool ShouldSerializetournament() => useTournament;

    }

    public class MapQueryFilter {
        public MapQueryFilter(bool useName, bool useI18n, bool useHash,
        string name = default, string i18n = default, string hash = default) {
            this.name = name;
            this.i18n = i18n;
            this.hash = hash;
            this.useName = useName;
            this.useI18n = useI18n;
            this.useHash = useHash;
        }

        public string name { get; set; }
        public string i18n { get; set; }
        public string hash { get; set; }
        private bool useName { get; set; }
        private bool useI18n { get; set; }
        private bool useHash { get; set; }

        public bool ShouldSerializename() => useName;
        public bool ShouldSerializei18n() => useI18n;
        public bool ShouldSerializehash() => useHash;

    }

    #endregion

    #region new value (add)

    public class UserAddBuilder {
        public UserAddBuilder(string name, string password, SM_Priority priority) {
            this.name = name;
            this.password = HashComput.SHA256FromString(password);
            this.priority = priority;
        }

        public string name { get; set; }
        public string password { get; set; }
        public SM_Priority priority { get; set; }

    }

    public class CompetitionAddBuilder {
        public CompetitionAddBuilder(long startDate, long endDate, long judgeEndDate, List<string> participant) {
            this.startDate = startDate;
            this.endDate = endDate;
            this.judgeEndDate = judgeEndDate;
            this.participant = participant;
        }

        public long startDate { get; set; }
        public long endDate { get; set; }
        public long judgeEndDate { get; set; }
        public List<string> participant { get; set; }

    }

    public class TournamentAddBuilder {
        public TournamentAddBuilder(long startDate, long endDate, string name) {
            this.startDate = startDate;
            this.endDate = endDate;
            this.name = name;
        }

        public long startDate { get; set; }
        public long endDate { get; set; }
        public string name { get; set; }

    }

    public class RegistryAddDeleteBuilder {
        public RegistryAddDeleteBuilder(string user, string tournament) {
            this.user = user;
            this.tournament = tournament;
        }

        public string user { get; set; }
        public string tournament { get; set; }

    }

    public class MapPoolAddDeleteBuilder {
        public MapPoolAddDeleteBuilder(string hash, string tournament) {
            this.hash = hash;
            this.tournament = tournament;
        }

        public string hash { get; set; }
        public string tournament { get; set; }

    }

    public class MapAddBuilder {
        public MapAddBuilder(string name, string i18n, string hasn) {
            this.name = name;
            this.i18n = i18n;
            this.hasn = hasn;
        }

        public string name { get; set; }
        public string i18n { get; set; }
        public string hasn { get; set; }

    }

    #endregion

}

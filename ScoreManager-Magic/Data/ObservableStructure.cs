using SMMLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScoreManager_Magic.Data {

    public class ObservableGetFutureCompetition : SMMLib.Data.SMMStructure.GetFutureCompetition {
        public ObservableGetFutureCompetition(SMMLib.Data.SMMStructure.GetFutureCompetition root) {
            this.sm_id = root.sm_id;
            this.sm_startDate = root.sm_startDate;
            this.sm_endDate = root.sm_endDate;
            this.sm_map = root.sm_map;
            this.sm_cdk = root.sm_cdk;
            this.sm_participant = root.sm_participant;
        }

        public string conv_startDate {
            get {
                return sm_startDate.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_endDate {
            get {
                return sm_endDate.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_participant {
            get {
                return string.Join(", ", sm_participant);
            }
            set {; }
        }

        public string conv_map { get; set; }
    }

    public class ObservableGetTournament : SMMLib.Data.SMMStructure.GetTournament {
        public ObservableGetTournament(SMMLib.Data.SMMStructure.GetTournament root) {
            this.sm_tournament = root.sm_tournament;
            this.sm_startDate = root.sm_startDate;
            this.sm_endDate = root.sm_endDate;
            this.participanted = root.participanted;
        }

        public string conv_startDate {
            get {
                return sm_startDate.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_endDate {
            get {
                return sm_endDate.ConvertToDatetime().ToLocalTime().ToString();
            }
            set {; }
        }

        public string conv_participanted {
            get {
                return participanted.ToString();
            }
            set {; }
        }

    }

    public class ObservableBsmData {

        public ObservableBsmData(ScoreManager_Magic.Core.BsmData root, string map, DateTime dt) {
            sm_installOn = root.InstallOn.ToString();
            conv_map = map;
            sm_score = root.HSScore.ToString();
            conv_srTime = string.Format("{0}:{1}.{2}",
                                        (root.SRTime / (1000 * 60)).ToString(),
                                        (root.SRTime % (1000 * 60) / 1000).ToString().PadLeft(2, '0'),
                                        (root.SRTime % 1000).ToString().PadLeft(4, '0'));
            sm_lifeUp = root.LifeUp.ToString();
            sm_lifeLost = root.LifeLost.ToString();
            sm_extraPoint = root.ExtraPoints.ToString();
            sm_subExtraPoint = root.SubExtraPoints.ToString();
            sm_trafo = root.Trafo.ToString();
            sm_checkpoint = root.Checkpoint.ToString();
            conv_date = dt.ToString();
        }

        public string sm_installOn { get; set; }
        public string conv_map { get; set; }
        public string sm_score { get; set; }
        public string conv_srTime { get; set; }
        public string sm_lifeUp { get; set; }
        public string sm_lifeLost { get; set; }
        public string sm_extraPoint { get; set; }
        public string sm_subExtraPoint { get; set; }
        public string sm_trafo { get; set; }
        public string sm_checkpoint { get; set; }
        public string conv_date { get; set; }
    }

}


using System.Runtime.Serialization;

namespace ivNet.Club.ViewModel
{
    [DataContract]
    public class PlayerStatViewModel
    {
        [DataMember]
        public int FixtureId { get; set; }
        [DataMember]
        public string PlayerName { get; set; }
        [DataMember]
        public string PlayerNumber { get; set; }
        [DataMember]
        public int Captain { get; set; }
        [DataMember]
        public int Catches { get; set; }
        [DataMember]
        public int Innings { get; set; }
        [DataMember]
        public int Keeper { get; set; }
        [DataMember]
        public int Maidens { get; set; }
        [DataMember]
        public int HowOut { get; set; }
        [DataMember]
        public int Overs { get; set; }
        [DataMember]
        public int Runs { get; set; }
        [DataMember]
        public int RunsConceeded { get; set; }
        [DataMember]
        public int Wickets { get; set; }
        [DataMember]
        public int HighestScore { get; set; }
        [DataMember]
        public int Fifties { get; set; }
        [DataMember]
        public int Hundreds { get; set; }

        [DataMember]
        public string BattingAverage
        {
            get { return (Innings - HowOut) == 0 ? "0" : string.Format("{0:.##}", Runs/(Innings - HowOut)); }
            private set { }
        }

        [DataMember]
        public string BowlingAverage
        {
            get { return Wickets == 0 ? "0" : string.Format("{0:.##}", RunsConceeded/Wickets); }
            private set { }
        }

        [DataMember]
        public string StrikeRate
        {
            get { return Wickets == 0 ? "0" : string.Format("{0:.##}", (Overs * 6) / Wickets); }
            private set { }
        }

        [DataMember]
        public string EconomyRate
        {
            get { return Overs == 0 ? "0" : string.Format("{0:.##}", RunsConceeded / Overs); }
            private set { }
        }
    }
}
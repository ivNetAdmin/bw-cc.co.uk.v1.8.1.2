
using System;

namespace ivNet.Club.ViewModel
{
    public class BWCCLoadFixtureViewModel
    {
        public int LegacyFixtureId { get; set; }
        public int LegacyVenueId { get; set; }
        public string Venue { get; set; }
        public DateTime DatePlayed { get; set; }
        public int LegacyTeamId { get; set; }
        public string Team { get; set; }
        public int OppisitionId { get; set; }
        public string Oppisition { get; set; }
        public int LegacyFixtureTypeId { get; set; }
        public string FixtureType { get; set; }
        public int ResultTypeId { get; set; }
        public string ResultType { get; set; }
        public string Score { get; set; }
    }
}
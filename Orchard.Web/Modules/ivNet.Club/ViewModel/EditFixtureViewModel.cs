
using System;

namespace ivNet.Club.ViewModel
{
    public class EditFixtureViewModel
    {
        public int LegacyFixtureId { get; set; }
        public int LegacyVenueId { get; set; }
        public string Venue { get; set; }
        public DateTime DatePlayed { get; set; }
        public int LegacyTeamId { get; set; }
        public string Team { get; set; }
    }
}

using System.Collections.Generic;
using ivNet.Club.Entities;

namespace ivNet.Club.ViewModel
{
    public class AdminFixtureReportViewModel
    {
        public IEnumerable<FixtureResult> Results { get; set; }
        public int FixtureId { get; set; }
        public int FixtureResult { get; set; }
        public string FixtureScore { get; set; }
        public string MatchReport { get; set; }        
    }
}
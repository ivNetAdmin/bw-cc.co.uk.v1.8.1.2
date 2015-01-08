
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class FixtureConfigViewModel
    {
        public string Type { get; set; }
        public List<FixtureItemConfigViewModel> Teams { get; set; }
        public List<FixtureItemConfigViewModel> Opponents { get; set; }
        public List<FixtureItemConfigViewModel> FixtureTypes { get; set; }
        public List<FixtureItemConfigViewModel> FixtureResults { get; set; }
        public List<FixtureItemConfigViewModel> Locations { get; set; }
        public List<FixtureItemConfigViewModel> HowOut { get; set; }
    }
}
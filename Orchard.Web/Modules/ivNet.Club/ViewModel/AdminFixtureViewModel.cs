
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class AdminFixtureViewModel
    {
        public AdminFixtureViewModel()
        {
            Fixtures = new List<FixtureViewModel>();
            Teams = new List<TeamViewModel>();
            Opponents = new List<OpponentViewModel>();
            Locations = new List<LocationViewModel>();
            FixtureTypes = new List<FixtureTypeViewModel>();
            ResultTypes = new List<ResultTypeViewModel>();
            //HowOut = new List<HowOutViewModel>();
            HomeOrAway = new List<string>();
        }

        public List<ResultTypeViewModel> ResultTypes { get; set; }
        public List<FixtureViewModel> Fixtures { get; set; }
        public List<TeamViewModel> Teams { get; set; }
        public List<OpponentViewModel> Opponents { get; set; }
        public List<LocationViewModel> Locations { get; set; }
        public List<FixtureTypeViewModel> FixtureTypes { get; set; }
        public List<string> HomeOrAway { get; set; }
       // public List<HowOutViewModel> HowOut { get; set; }
    }
}
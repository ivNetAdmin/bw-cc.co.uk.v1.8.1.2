
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class EditFixtureViewModel
    {
        public EditFixtureViewModel()
        {
            Fixtures = new List<FixtureViewModel>();
            Teams = new List<TeamViewModel>();
            Opponents = new List<OpponentViewModel>();
            Locations = new List<LocationViewModel>();
            FixtureTypes = new List<FixtureTypeViewModel>();            
        }

        public List<FixtureViewModel> Fixtures { get; set; }
        public List<TeamViewModel> Teams { get; set; }
        public List<OpponentViewModel> Opponents { get; set; }
        public List<LocationViewModel> Locations { get; set; }
        public List<FixtureTypeViewModel> FixtureTypes { get; set; }
    }
}

using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class EditFixtureViewModel
    {
        public EditFixtureViewModel()
        {
            Fixtures = new List<FixtureViewModel>();
            Teams = new List<TeamViewModel>();
        }

        public List<FixtureViewModel> Fixtures { get; set; }
        public List<TeamViewModel> Teams { get; set; }
        public List<OpponentViewModel> Opponents { get; set; }
        public List<FixtureTypeViewModel> FixtureTypes { get; set; }
    }
}
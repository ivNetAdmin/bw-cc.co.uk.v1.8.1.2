
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class FixtureListViewModel
    {
        public FixtureListViewModel()
        {
            Fixtures = new List<FixtureViewModel>();
        }

        public List<FixtureViewModel> Fixtures { get; set; }        
    }
}
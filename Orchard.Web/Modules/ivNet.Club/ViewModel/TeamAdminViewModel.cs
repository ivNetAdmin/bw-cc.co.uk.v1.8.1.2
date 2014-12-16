
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class TeamAdminViewModel
    {
        public TeamAdminViewModel()
        {
            AdminFixtureViewModel=new AdminFixtureViewModel();
            Players=new List<PlayerViewModel>();
            TeamSelection = new List<PlayerViewModel>();
            for (var i = 0; i < 13; i++)
            {
                TeamSelection.Add(new PlayerViewModel());
            }
        }

        public AdminFixtureViewModel AdminFixtureViewModel { get; set; }
        public List<PlayerViewModel> Players { get; set; }
        public List<PlayerViewModel> TeamSelection { get; set; }
    }
}
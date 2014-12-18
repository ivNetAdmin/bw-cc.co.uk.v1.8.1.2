
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class TeamSelectionAdminViewModel
    {
        public TeamSelectionAdminViewModel ()
        {
         TeamSelection=new List<PlayerViewModel>();   
        }

        public int FixtureId { get; set; }
        public List<PlayerViewModel> TeamSelection { get; set; }
    }
}
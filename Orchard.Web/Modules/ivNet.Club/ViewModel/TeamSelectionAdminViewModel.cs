
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class TeamSelectionAdminViewModel
    {
        public TeamSelectionAdminViewModel ()
        {
         TeamSelection=new List<PlayerViewModel>();   
        }

        public List<PlayerViewModel> TeamSelection { get; set; }
    }
}
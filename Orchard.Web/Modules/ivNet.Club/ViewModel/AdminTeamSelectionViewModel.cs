
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class AdminTeamSelectionViewModel
    {
        public AdminTeamSelectionViewModel()
        {
           TeamSelection = new List<PlayerViewModel>();          
        }

        public int TeamSelectionId { get; set; }
        public List<PlayerViewModel> TeamSelection { get; set; }
    }
}
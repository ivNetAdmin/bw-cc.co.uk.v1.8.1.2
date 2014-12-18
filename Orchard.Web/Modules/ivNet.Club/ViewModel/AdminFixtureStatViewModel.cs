
using System.Collections.Generic;
using NHibernate.Mapping;

namespace ivNet.Club.ViewModel
{
    public class AdminFixtureStatViewModel
    {
        public AdminFixtureStatViewModel()
        {
            PlayerStats=new List<PlayerStatViewModel>();
        }

        public List<PlayerStatViewModel> PlayerStats { get; set; }
    }
}

using System.Collections.Generic;
using NHibernate.Mapping;

namespace ivNet.Club.ViewModel
{
    public class RegistrationUpdateViewModel
    {
        public RegistrationUpdateViewModel()
        {
            Guardians=new List<RegistrationViewModel>();
            Juniors = new List<JuniorViewModel>();
        }

        public List<RegistrationViewModel> Guardians { get; set; }
        public List<JuniorViewModel> Juniors { get; set; }
    }
}
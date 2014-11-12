
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class RegistrationViewModel
    {
        public MemberViewModel MemberViewModel { get; set; }
        public ContactViewModel ContactViewModel { get; set; }
        public AddressViewModel AddressViewModel { get; set; }
        public List<JuniorViewModel> JuniorList { get; set; }
        public string Season { get; set; }

        public RegistrationViewModel()
        {
            JuniorList = new List<JuniorViewModel>();
            MemberViewModel = new MemberViewModel();
            ContactViewModel = new ContactViewModel();
            AddressViewModel = new AddressViewModel();
        }
    }
}
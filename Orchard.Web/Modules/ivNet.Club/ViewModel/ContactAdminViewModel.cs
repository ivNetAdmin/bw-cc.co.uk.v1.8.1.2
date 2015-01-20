
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class ContactAdminViewModel
    {
        public ContactAdminViewModel()
        {
            Contacts=new List<MemberViewModel>();
            JuniorContacts=new List<JuniorContactViewModel>();
        }

        public List<MemberViewModel> Contacts { get; set; }
        public List<JuniorContactViewModel> JuniorContacts { get; set; }
    }
}
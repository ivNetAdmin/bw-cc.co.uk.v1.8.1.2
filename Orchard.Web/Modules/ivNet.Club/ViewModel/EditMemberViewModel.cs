
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class EditMemberViewModel
    {
        public EditMemberViewModel()
        {
            Guardians=new List<MemberViewModel>();
            Juniors = new List<MemberViewModel>();
            NewGuardian=new MemberViewModel();
            NewJunior = new MemberViewModel();
        }
        public string Type { get; set; }
        public int MemberType { get; set; }
        public string AuthenticatedUser { get; set; }

        public string Season { get; set; }
        
        public List<MemberViewModel> Guardians { get; set; }
        public List<MemberViewModel> Juniors { get; set; }
        public MemberViewModel NewGuardian { get; set; }
        public MemberViewModel NewJunior { get; set; }
        
    }
}
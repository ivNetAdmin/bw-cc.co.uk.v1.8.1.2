
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class EditMemberViewModel
    {
        public EditMemberViewModel()
        {
            Guardians=new List<_MemberViewModel>();
            Juniors = new List<_MemberViewModel>();
            NewGuardian=new _MemberViewModel();
            NewJunior = new _MemberViewModel();
        }
        public string Type { get; set; }
        public int MemberType { get; set; }
        public string Season { get; set; }
        public List<_MemberViewModel> Guardians { get; set; }
        public List<_MemberViewModel> Juniors { get; set; }
        public _MemberViewModel NewGuardian { get; set; }
        public _MemberViewModel NewJunior { get; set; }
    }
}
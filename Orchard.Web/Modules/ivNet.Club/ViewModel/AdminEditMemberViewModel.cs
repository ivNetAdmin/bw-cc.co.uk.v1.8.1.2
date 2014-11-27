
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class AdminEditMemberViewModel
    {
        public AdminEditMemberViewModel()
        {
            Guardians=new List<_MemberViewModel>();
            Juniors = new List<_MemberViewModel>();
        }
        public int MemberType { get; set; }
        public List<_MemberViewModel> Guardians { get; set; }
        public List<_MemberViewModel> Juniors { get; set; }
    }
}
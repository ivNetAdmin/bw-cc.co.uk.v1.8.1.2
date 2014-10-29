
using System;

namespace ivNet.Club.ViewModel
{
    public class JuniorViewModel
    {
        public MemberViewModel MemberViewModel { get; set; }

        public string JuniorKey { get; set; }
        public DateTime Dob { get; set; }

        public string School { get; set; }
        public string Team { get; set; }
        public string Notes { get; set; }

        public string ShirtSize { get; set; }
        public string ShortSize { get; set; }
        public string BootSize { get; set; }

        public JuniorViewModel()
        {
            MemberViewModel = new MemberViewModel();
        }
    }
}

using System;
using System.Collections.Generic;

namespace ivNet.Club.ViewModel
{
    public class RegisteredGuardianViewModel
    {
        public RegisteredGuardianViewModel()
        {
            MemberDetails = new List<MemberDetailViewModel>();
            JuniorList = new List<JuniorDetailViewModel>();
            NewMemberDetails = new List<MemberDetailViewModel>{new MemberDetailViewModel()};
            NewJuniorList = new List<JuniorDetailViewModel> {new JuniorDetailViewModel()};
        }

        public List<MemberDetailViewModel> MemberDetails { get; set; }
        public List<JuniorDetailViewModel> JuniorList { get; set; }
        public List<MemberDetailViewModel> NewMemberDetails { get; set; }
        public List<JuniorDetailViewModel> NewJuniorList { get; set; }
    }

    public class MemberDetailViewModel
    {
        public int MemberId { get; set; }
        public string MemberKey { get; set; }        
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Nickname { get; set; }
        public string Mobile { get; set; }
        public string OtherTelephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }      
        public byte IsActive { get; set; }

        public string Fullname
        {
            get { return string.Format("{0} {1}", Firstname, Surname); }
        }

        public string MemberNo
        {
            get { return string.Format("{0:00000}", MemberId); }
        }
    }

    public class JuniorDetailViewModel
    {
        public int MemberId { get; set; }
        public string MemberKey { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Nickname { get; set; }
        public DateTime Dob { get; set; }
        public string School { get; set; }
        public string Notes { get; set; }
        public byte IsActive { get; set; }

        public string Fullname
        {
            get { return string.Format("{0} {1}", Firstname, Surname); }
        }

        public string MemberNo
        {
            get { return string.Format("{0:00000}", MemberId); }
        }

    }
}
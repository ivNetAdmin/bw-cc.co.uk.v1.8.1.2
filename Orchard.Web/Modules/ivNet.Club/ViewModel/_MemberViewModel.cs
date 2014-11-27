
using System;

namespace ivNet.Club.ViewModel
{
    public class _MemberViewModel
    {
        public int MemberId { get; set; }
        public string MemberKey { get; set; }

        public string Surname { get; set; }
        public string Firstname { get; set; }

        public string Nickname { get; set; }

        public string Email { get; set; }     
        public DateTime? Dob { get; set; }       
        public byte IsActive { get; set; }
    }
}
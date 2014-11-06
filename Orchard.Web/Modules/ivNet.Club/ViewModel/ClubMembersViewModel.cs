using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ivNet.Club.ViewModel
{
    public class ClubMembersViewModel
    {
        public decimal Fee { get; set; }
        public int MemberId { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public DateTime? Dob { get; set; }
        public string MemberType { get; set; }
        public byte IsActive { get; set; }
    }
}
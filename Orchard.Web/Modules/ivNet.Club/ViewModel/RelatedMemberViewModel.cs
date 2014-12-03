
using System;
using System.Collections.Generic;
using ivNet.Club.Enums;

namespace ivNet.Club.ViewModel
{
    public class RelatedMemberViewModel
    {
        public RelatedMemberViewModel()
        {
            RelatedMembeList=new List<string>();
        }

        public string MemberKey { get; set; }
        public int MemberType { get; set; }

        public string Surname { get; set; }
        public string Firstname { get; set; }

        public string Nickname { get; set; }

        public int MemberId { get; set; }

        public DateTime? Dob { get; set; }
        public byte IsActive { get; set; }

        public List<string> RelatedMembeList { get; set; }
    }
}
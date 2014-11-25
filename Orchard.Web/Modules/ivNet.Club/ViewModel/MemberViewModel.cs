using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ivNet.Club.ViewModel
{
    public class MemberViewModel
    {        
        public string MemberKey { get; set; }

        public string Surname { get; set; }
        public string Firstname { get; set; }

        public string Nickname { get; set; }
        public string Fee { get; set; }
        public int MemberId { get; set; }
      
        public DateTime? Dob { get; set; }
        public string MemberType { get; set; }
        public byte IsActive { get; set; }
    
    }
}
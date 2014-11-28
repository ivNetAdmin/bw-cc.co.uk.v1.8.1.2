
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

        public string ContactDetailKey { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string OtherTelephone { get; set; }

        public string AddressDetailKey { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }

        public string School { get; set; }
        public string Team { get; set; }
        public string Notes { get; set; }

        public string BootSize { get; set; }
        public string ShirtSize { get; set; }
        public string ShortSize { get; set; }

        public DateTime Dob { get; set; }       
        public byte IsActive { get; set; }
        
    }
}
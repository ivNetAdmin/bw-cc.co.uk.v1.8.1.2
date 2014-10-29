﻿
using ivNet.Club.Helpers;

namespace ivNet.Club.ViewModel
{
    public class NewMembershipViewModel
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Nickname { get; set; }
        public string Role { get; set; }

        public string ClubMemberKey
        {
            get { return CustomStringHelper.BuildKey(new[] { Email }); }
        }

        public string Address { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        public string ContactKey 
        {
            get { return CustomStringHelper.BuildKey(new[] { Address, Postcode }); }
        }
        
    }
}
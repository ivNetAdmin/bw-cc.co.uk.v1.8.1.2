
using System;
using System.Collections.Generic;
using NHibernate.Mapping;

namespace ivNet.Club.ViewModel
{
    public class JuniorVettingViewModel
    {
        public JuniorVettingViewModel()
        {
            Guardians=new List<GuardianViewModel>();
        }

        public int JuniorId { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public DateTime Dob { get; set; }
        public string AgeGroup { get; set; }
        public byte IsVetted { get; set; }
        public List<GuardianViewModel> Guardians { get; set; }
    }
}
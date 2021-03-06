﻿
using System.Collections.Generic;
using System.Linq;
using ivNet.Club.Entities;
using ivNet.Club.Enums;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using NHibernate.Mapping;
using Orchard;
using Orchard.Security;

namespace ivNet.Club.Services
{
    public interface IGuardianServices : IDependency
    {
        //RegistrationViewModel GetByUserId(int id);
        //EditMemberViewModel GetRegisteredUser();
        //GuardianViewModel GetByEmail(string email);
    }

    public class GuardianServices : IGuardianServices
    {
        private readonly IAuthenticationService _authenticationService;

        public GuardianServices(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        //public RegistrationViewModel GetByUserId(int id)
        //{
        //    using (var session = NHibernateHelper.OpenSession())
        //    {
        //        var registrationViewModel = new RegistrationViewModel();

        //        var guardian = session.CreateCriteria(typeof(Guardian))
        //            .List<Guardian>().FirstOrDefault(x => x.Member.UserId.Equals(id));
        //        if (guardian != null)
        //        {
        //            MapperHelper.Map(registrationViewModel.MemberViewModel, guardian.Member);
        //            foreach (var junior in guardian.Juniors)
        //            {
        //                var juniorViewModel = new JuniorViewModel();
        //                juniorViewModel = MapperHelper.Map(juniorViewModel, junior);
        //                registrationViewModel.JuniorList.Add(juniorViewModel);
        //            }
        //        }

        //        return registrationViewModel;
        //    }
        //}

        //public EditMemberViewModel GetRegisteredUser()
        //{
        //    var user = _authenticationService.GetAuthenticatedUser();

        //    using (var session = NHibernateHelper.OpenSession())
        //    {
        //        var editMemberViewModel = new EditMemberViewModel {MemberType = (int) MemberType.Guardian};

        //        var guardian = session.CreateCriteria(typeof(Guardian))
        //            .List<Guardian>().FirstOrDefault(x => x.Member.UserId.Equals(user.Id));

        //        if (guardian != null)
        //        {

        //            var associatedGuardianList = guardian.Juniors[0].Guardians;

        //            foreach (var associatedGuardian in associatedGuardianList)
        //            {
        //                var guardianViewModel = new _MemberViewModel();
        //                MapperHelper.Map(guardianViewModel, associatedGuardian.Member);
        //                MapperHelper.Map(guardianViewModel, associatedGuardian.AddressDetail);
        //                MapperHelper.Map(guardianViewModel, associatedGuardian.ContactDetail);
        //                editMemberViewModel.Guardians.Add(guardianViewModel);                        
        //            }
                  
        //            foreach (var junior in guardian.Juniors)
        //            {
        //                var juniorViewModel = new _MemberViewModel();
        //                MapperHelper.Map(juniorViewModel, junior.Member);
        //                MapperHelper.Map(juniorViewModel, junior.JuniorInfo);
        //                juniorViewModel.Dob = junior.Dob;
        //                editMemberViewModel.Juniors.Add(juniorViewModel);
        //            }
        //        }

        //        return editMemberViewModel;
        //    }
        //}
       
        //public GuardianViewModel GetByEmail(string email)
        //{
        //    using (var session = NHibernateHelper.OpenSession())
        //    {
        //        var guardianViewModel = new GuardianViewModel();

        //        var guardian = session.CreateCriteria(typeof (Guardian))
        //            .List<Guardian>().FirstOrDefault(x => x.ContactDetail.Email.Equals(email));

        //        return guardian == null ? null : MapperHelper.Map(guardianViewModel, guardian);
        //    }
        //}       
    }
}
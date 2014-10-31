
using System.Collections.Generic;
using System.Globalization;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Data;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;

namespace ivNet.Club.Services
{
    public interface IClubMemberServices : IDependency
    {
        void CreateNewMember(NewMembershipViewModel viewModel);

        void CreateGuardian(List<RegistrationViewModel> registrationList);
    }

    public class ClubMemberServices : BaseService, IClubMemberServices
    {
        private readonly IRegistrationServices _registrationServices;

        public ClubMemberServices(
            IMembershipService membershipService,
            IAuthenticationService authenticationService,
            IRoleService roleService,
            IRepository<UserRolesPartRecord> userRolesRepository,
            IRegistrationServices registrationServices)
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
            _registrationServices = registrationServices;
        }

        public void CreateNewMember(NewMembershipViewModel viewModel)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                   
                    transaction.Commit();
                }
            }
        }

        public void CreateGuardian(List<RegistrationViewModel> registrationList)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    _registrationServices.Clear();

                    foreach (var registrationViewModel in registrationList)
                    {
                        var guardian = new Guardian();
                        guardian.Init();
                        guardian = DuplicateCheck(session, guardian, registrationViewModel.MemberViewModel.ClubMemberKey);

                        if (guardian.Id == 0)
                            guardian.GuardianKey = registrationViewModel.MemberViewModel.ClubMemberKey;

                        // check data has not already been saved
                        guardian.ClubMember = DuplicateCheck(session, guardian.ClubMember,
                            registrationViewModel.MemberViewModel.ClubMemberKey);

                        guardian.ContactDetail = DuplicateCheck(session, guardian.ContactDetail,
                            registrationViewModel.ContactViewModel.ContactDetailKey);

                        // update width new user details
                        MapperHelper.Map(guardian.ClubMember, registrationViewModel.MemberViewModel);
                        MapperHelper.Map(guardian.ContactDetail, registrationViewModel.ContactViewModel);

                        // create website account 
                        if (guardian.ClubMember.UserId == 0)
                        {
                            guardian.ClubMember.UserId =
                                CreateAccount(guardian.ClubMember,
                                    registrationViewModel.ContactViewModel.Email);
                        }

                        // save or update guardian elements
                        SetAudit(guardian.ClubMember);
                        session.SaveOrUpdate(guardian.ClubMember);
                        SetAudit(guardian.ContactDetail);
                        session.SaveOrUpdate(guardian.ContactDetail);

                        // add junior details
                        foreach (var juniorViewModel in registrationViewModel.JuniorList)
                        {
                            var junior = new Junior();
                            junior.Init();
                            junior = DuplicateCheck(session, junior, juniorViewModel.MemberViewModel.ClubMemberKey);

                            if (junior.Id == 0)
                                junior.JuniorKey = juniorViewModel.MemberViewModel.ClubMemberKey;

                            junior.Dob = juniorViewModel.Dob;

                            // check data has not already been saved
                            junior.ClubMember = DuplicateCheck(session, junior.ClubMember,
                                juniorViewModel.MemberViewModel.ClubMemberKey);

                            // update width new user details
                            MapperHelper.Map(junior.ClubMember, juniorViewModel.MemberViewModel);
                            MapperHelper.Map(junior.JuniorInfo, juniorViewModel);

                            // create website account
                            if (junior.ClubMember.UserId == 0)
                            {
                                junior.ClubMember.UserId =
                                    CreateAccount(junior.ClubMember,
                                        registrationViewModel.ContactViewModel.Email);
                            }

                            // save or update junior elements
                            SetAudit(junior.ClubMember);
                            session.SaveOrUpdate(junior.ClubMember);
                            SetAudit(junior.JuniorInfo);
                            session.SaveOrUpdate(junior.JuniorInfo);                       

                            //var player = new Player();
                            //player = DuplicateCheck(session, player, junior.ClubMember);

                            
                            junior.Player = new Player { Number = junior.ClubMember.Id.ToString(CultureInfo.InvariantCulture).PadLeft(6, '0') };
                            
                            junior.Player.Init();
                            MapperHelper.Map(junior.Player.Kit, juniorViewModel);

                            SetAudit(junior.Player.Kit);
                            session.SaveOrUpdate(junior.Player.Kit);         

                            SetAudit(junior.Player);
                            session.SaveOrUpdate(junior.Player);
                       
                            // save or update junior
                            SetAudit(junior);
                            session.SaveOrUpdate(junior);

                            _registrationServices.Add(junior.ClubMember);

                            guardian.AddJunior(junior);
                        }

                        // save or update guardian
                        SetAudit(guardian);
                        session.SaveOrUpdate(guardian);
                    }

                    transaction.Commit();
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using NHibernate;
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
        List<ClubMembersViewModel> GetAll();
        List<ClubMembersViewModel> Get(int id);
        List<GuardianViewModel> GetGuardians(int id);
        MemberViewModel GetMember(string email);
    }

    public class ClubMemberServices : BaseService, IClubMemberServices
    {
        private readonly IRegistrationServices _registrationServices;
        private readonly IConfigurationServices _configurationServices;

        public ClubMemberServices(
            IMembershipService membershipService,
            IAuthenticationService authenticationService,
            IRoleService roleService,
            IRepository<UserRolesPartRecord> userRolesRepository,
            IRegistrationServices registrationServices,
            IConfigurationServices configurationServices)
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
            _registrationServices = registrationServices;
            _configurationServices = configurationServices;
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

                        guardian.AddressDetail = DuplicateCheck(session, guardian.AddressDetail,
                            registrationViewModel.AddressViewModel.AddressDetailKey);

                        // update width new user details
                        MapperHelper.Map(guardian.ClubMember, registrationViewModel.MemberViewModel);
                        MapperHelper.Map(guardian.ContactDetail, registrationViewModel.ContactViewModel);
                        MapperHelper.Map(guardian.AddressDetail, registrationViewModel.AddressViewModel);

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
                        SetAudit(guardian.AddressDetail);
                        session.SaveOrUpdate(guardian.AddressDetail);

                        // add junior details
                        foreach (var juniorViewModel in registrationViewModel.JuniorList)
                        {
                            var junior = new Junior();
                            junior.Init();
                            junior.Player.Init();

                            junior = DuplicateCheck(session, junior, juniorViewModel.MemberViewModel.ClubMemberKey);

                            if (junior.Id == 0)
                            {
                                junior.JuniorGuardianKey = juniorViewModel.MemberViewModel.ClubMemberKey;
                                junior.JuniorKey =
                                    CustomStringHelper.BuildKey(new[]
                                    {
                                        juniorViewModel.MemberViewModel.Firstname,
                                        juniorViewModel.MemberViewModel.Surname
                                    });
                            }

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

                            MapperHelper.Map(junior.Player.Kit, juniorViewModel);
                            SetAudit(junior.Player.Kit);
                            session.SaveOrUpdate(junior.Player.Kit);

                            // create a blank fee for this season
                            var fee = new Fee {Season = registrationViewModel.Season};
                            SetAudit(fee);
                            session.SaveOrUpdate(fee);
                            junior.Player.Fees.Add(fee);

                            junior.Player.Number = junior.ClubMember.Id.ToString(CultureInfo.InvariantCulture)
                                .PadLeft(6, '0');

                            SetAudit(junior.Player);
                            session.SaveOrUpdate(junior.Player);

                            // save or update junior
                            SetAudit(junior);
                            session.SaveOrUpdate(junior);

                            _registrationServices.Add(junior.ClubMember.Id);

                            guardian.AddJunior(junior);
                        }

                        // save or update guardian
                        SetAudit(guardian);
                        session.SaveOrUpdate(guardian);

                        // add fees for this season
                        AddFee(session, guardian.Juniors);
                    }

                    transaction.Commit();
                }
            }
        }

        private void AddFee(ISession session, IList<Junior> juniors)
        {

        }

        public List<ClubMembersViewModel> GetAll()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var returnList = new List<ClubMembersViewModel>();

                var guardianList = session.CreateCriteria(typeof (Guardian))
                    .List<Guardian>().Where(x => x.IsActive.Equals(1)).ToList();

                var guardians = (from guardian in guardianList
                    let clubMembersViewModel = new ClubMembersViewModel()
                    select MapperHelper.Map(clubMembersViewModel, guardian)).ToList();

                var juniorList = session.CreateCriteria(typeof (Junior))
                    .List<Junior>().Where(x => x.IsActive.Equals(1)).ToList();

                var juniors = (from junior in juniorList
                    let clubMembersViewModel = new ClubMembersViewModel()
                    select MapperHelper.Map(_configurationServices, clubMembersViewModel, junior)).ToList();

                returnList.AddRange(guardians);
                returnList.AddRange(juniors);

                return returnList;
            }
        }

        public List<ClubMembersViewModel> Get(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var returnList = new List<ClubMembersViewModel>();

                var guardianList = session.CreateCriteria(typeof (Guardian))
                    .List<Guardian>().Where(x => x.IsActive.Equals(1) && x.ClubMember.Id.Equals(id)).ToList();

                var guardians = (from guardian in guardianList
                    let clubMembersViewModel = new ClubMembersViewModel()
                    select MapperHelper.Map(clubMembersViewModel, guardian)).ToList();

                var juniorList = session.CreateCriteria(typeof (Junior))
                    .List<Junior>().Where(x => x.IsActive.Equals(1) && x.ClubMember.Id.Equals(id)).ToList();

                var juniors = (from junior in juniorList
                    let clubMembersViewModel = new ClubMembersViewModel()
                    select MapperHelper.Map(_configurationServices, clubMembersViewModel, junior)).ToList();

                returnList.AddRange(guardians);
                returnList.AddRange(juniors);

                return returnList;
            }
        }

        public List<GuardianViewModel> GetGuardians(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var junior = session.CreateCriteria(typeof (Junior))
                    .List<Junior>().First(x => x.ClubMember.Id.Equals(id));

                return (from guardian in junior.Guardians
                    let guardianViewModel = new GuardianViewModel()
                    select MapperHelper.Map(guardianViewModel, guardian)).ToList();
            }
        }

        public MemberViewModel GetMember(string email)
        {
            using (var session = NHibernateHelper.OpenSession())
            {

                var member = new MemberViewModel();
                var key = CustomStringHelper.BuildKey(new[] {email});
                var clubMember = session.CreateCriteria(typeof (ClubMember))
                    .List<ClubMember>().FirstOrDefault(x => x.IsActive.Equals(1) && x.ClubMemberKey.Equals(key));
                return MapperHelper.Map(member, clubMember);

            }
        }
    }
}
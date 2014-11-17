
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
    public interface IMemberServices : IDependency
    {
        void CreateNewMember(NewMemberViewModel viewModel);

        void CreateGuardian(List<RegistrationViewModel> registrationList);
        List<MemberViewModel> GetAll();
        List<MemberViewModel> Get(int id);
        List<GuardianViewModel> GetGuardians(int id);
        MemberViewModel GetByEmail(string email);
        MemberViewModel GetByJuniorKey(string key);
        
        IUser AuthenticatedUser();
    }

    public class MemberServices : BaseService, IMemberServices
    {
        private readonly IRegistrationServices _registrationServices;
        private readonly IConfigurationServices _configurationServices;
        private readonly IAuthenticationService _authenticationService;

        public MemberServices(
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
            _authenticationService = authenticationService;
        }

        public void CreateNewMember(NewMemberViewModel viewModel)
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
                        guardian = DuplicateCheck(session, guardian, registrationViewModel.MemberViewModel.MemberKey);

                        if (guardian.Id == 0)
                            guardian.GuardianKey = registrationViewModel.MemberViewModel.MemberKey;

                        // check data has not already been saved
                        guardian.Member = DuplicateCheck(session, guardian.Member,
                            registrationViewModel.MemberViewModel.MemberKey);

                        guardian.ContactDetail = DuplicateCheck(session, guardian.ContactDetail,
                            registrationViewModel.ContactViewModel.ContactDetailKey);

                        guardian.AddressDetail = DuplicateCheck(session, guardian.AddressDetail,
                            registrationViewModel.AddressViewModel.AddressDetailKey);

                        // update width new user details
                        MapperHelper.Map(guardian.Member, registrationViewModel.MemberViewModel);
                        MapperHelper.Map(guardian.ContactDetail, registrationViewModel.ContactViewModel);
                        MapperHelper.Map(guardian.AddressDetail, registrationViewModel.AddressViewModel);

                        // create website account 
                        if (guardian.Member.UserId == 0)
                        {
                            guardian.Member.UserId =
                                CreateAccount(guardian.Member,
                                    registrationViewModel.ContactViewModel.Email, false);
                        }

                        // save or update guardian elements
                        SetAudit(guardian.Member);
                        session.SaveOrUpdate(guardian.Member);
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

                            junior = DuplicateCheck(session, junior, juniorViewModel.MemberViewModel.MemberKey);

                            if (junior.Id == 0)
                            {
                                junior.JuniorGuardianKey = juniorViewModel.MemberViewModel.MemberKey;
                                junior.JuniorKey =
                                    CustomStringHelper.BuildKey(new[]
                                    {
                                        juniorViewModel.MemberViewModel.Firstname,
                                        juniorViewModel.MemberViewModel.Surname
                                    });
                            }

                            junior.Dob = juniorViewModel.Dob;

                            // check data has not already been saved
                            junior.Member = DuplicateCheck(session, junior.Member,
                                juniorViewModel.MemberViewModel.MemberKey);

                            // update width new user details
                            MapperHelper.Map(junior.Member, juniorViewModel.MemberViewModel);
                            MapperHelper.Map(junior.JuniorInfo, juniorViewModel);

                            // create website account
                            if (junior.Member.UserId == 0)
                            {
                                junior.Member.UserId =
                                    CreateAccount(junior.Member,
                                        registrationViewModel.ContactViewModel.Email, true);
                            }

                            // save or update junior elements
                            SetAudit(junior.Member);
                            session.SaveOrUpdate(junior.Member);
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

                            junior.Player.Number = junior.Member.Id.ToString(CultureInfo.InvariantCulture)
                                .PadLeft(6, '0');

                            SetAudit(junior.Player);
                            session.SaveOrUpdate(junior.Player);

                            // save or update junior
                            SetAudit(junior);
                            session.SaveOrUpdate(junior);

                            _registrationServices.Add(junior.Member.Id);

                            guardian.AddJunior(junior);
                        }

                        // save or update guardian
                        SetAudit(guardian);
                        guardian.IsActive = 1;
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

        public List<MemberViewModel> GetAll()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var returnList = new List<MemberViewModel>();

                var guardianList = session.CreateCriteria(typeof (Guardian))
                    .List<Guardian>().Where(x => x.IsActive.Equals(1)).ToList();

                var guardians = (from guardian in guardianList
                    let memberViewModel = new MemberViewModel()
                    select MapperHelper.Map(memberViewModel, guardian)).ToList();

                var juniorList = session.CreateCriteria(typeof (Junior))
                    .List<Junior>().Where(x => x.IsActive.Equals(1)).ToList();

                var juniors = (from junior in juniorList
                    let memberViewModel = new MemberViewModel()
                    select MapperHelper.Map(_configurationServices, memberViewModel, junior)).ToList();

                returnList.AddRange(guardians);
                returnList.AddRange(juniors);

                return returnList;
            }
        }

        public List<MemberViewModel> Get(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var returnList = new List<MemberViewModel>();

                var guardianList = session.CreateCriteria(typeof (Guardian))
                    .List<Guardian>().Where(x => x.IsActive.Equals(1) && x.Member.Id.Equals(id)).ToList();

                var guardians = (from guardian in guardianList
                    let memberViewModel = new MemberViewModel()
                    select MapperHelper.Map(memberViewModel, guardian)).ToList();

                var juniorList = session.CreateCriteria(typeof (Junior))
                    .List<Junior>().Where(x => x.IsActive.Equals(1) && x.Member.Id.Equals(id)).ToList();

                var juniors = (from junior in juniorList
                    let memberViewModel = new MemberViewModel()
                    select MapperHelper.Map(_configurationServices, memberViewModel, junior)).ToList();

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
                    .List<Junior>().First(x => x.Member.Id.Equals(id));

                return (from guardian in junior.Guardians
                    let guardianViewModel = new GuardianViewModel()
                    select MapperHelper.Map(guardianViewModel, guardian)).ToList();
            }
        }

        public MemberViewModel GetByEmail(string email)
        {
            using (var session = NHibernateHelper.OpenSession())
            {

                var memberViewModel = new MemberViewModel();
                var key = CustomStringHelper.BuildKey(new[] {email});
                var member = session.CreateCriteria(typeof (Member))
                    .List<Member>().FirstOrDefault(x => x.IsActive.Equals(1) && x.MemberKey.Equals(key));
                return MapperHelper.Map(memberViewModel, member);

            }
        }

        public MemberViewModel GetByJuniorKey(string key)
        {
            throw new System.NotImplementedException();
        }

        public IUser AuthenticatedUser()
        {
            return _authenticationService.GetAuthenticatedUser();
        }
    }
}
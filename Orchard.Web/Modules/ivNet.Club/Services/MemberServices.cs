
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using FluentNHibernate.Utils;
using ivNet.Club.Entities;
using ivNet.Club.Enums;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using NHibernate;
using NHibernate.Mapping;
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

        void CreateGuardian(EditMemberViewModel registrationList);
        void UpdateGuardian(EditMemberViewModel registrationUpdateList);

        List<MemberViewModel> GetAll();
        EditMemberViewModel Get(int id);
        List<GuardianViewModel> GetGuardians(int id);
        MemberViewModel GetByKey(string key);
        MemberViewModel GetByUserId(int id);

        List<JuniorVettingViewModel> GetNonVetted();
        void Activate(int id, JuniorVettingViewModel item);
        
        IUser AuthenticatedUser();
        
    }

    public class MemberServices : BaseService, IMemberServices
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IConfigurationServices _configurationServices;
        private readonly IAuthenticationService _authenticationService;

        public MemberServices(
            IMembershipService membershipService,
            IAuthenticationService authenticationService,
            IRoleService roleService,
            IRepository<UserRolesPartRecord> userRolesRepository,
            IWorkContextAccessor workContextAccessor,
            IConfigurationServices configurationServices)
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
            _workContextAccessor = workContextAccessor;
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

        public void CreateGuardian(EditMemberViewModel editMemberViewModel)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    Clear();

                    foreach (var guardianViewModel in editMemberViewModel.Guardians)
                    {
                        var guardian = new Guardian();
                        guardian.Init();
                        guardian = DuplicateCheck(session, guardian, guardianViewModel.MemberKey);

                        if (guardian.Id == 0)
                            guardian.GuardianKey = guardianViewModel.MemberKey;

                        // check data has not already been saved
                        guardian.Member = DuplicateCheck(session, guardian.Member,
                            guardianViewModel.MemberKey);

                        guardian.ContactDetail = DuplicateCheck(session, guardian.ContactDetail,
                            guardianViewModel.ContactDetailKey);

                        guardian.AddressDetail = DuplicateCheck(session, guardian.AddressDetail,
                            guardianViewModel.AddressDetailKey);

                        // update width new user details
                        MapperHelper.Map(guardian.Member, guardianViewModel);
                        MapperHelper.Map(guardian.ContactDetail, guardianViewModel);
                        MapperHelper.Map(guardian.AddressDetail, guardianViewModel);

                        // create website account 
                        if (guardian.Member.UserId == 0)
                        {
                            guardian.Member.UserId =
                                CreateAccount(guardian.Member,
                                    guardianViewModel.Email, false);
                        }

                        // save or update guardian elements
                        SetAudit(guardian.Member);
                        session.SaveOrUpdate(guardian.Member);
                        SetAudit(guardian.ContactDetail);
                        session.SaveOrUpdate(guardian.ContactDetail);
                        SetAudit(guardian.AddressDetail);
                        session.SaveOrUpdate(guardian.AddressDetail);

                        // add junior details
                        foreach (var juniorViewModel in editMemberViewModel.Juniors)
                        {
                            var junior = new Junior();
                            junior.Init();
                            junior.Player.Init();

                            junior = DuplicateCheck(session, junior, juniorViewModel.MemberKey);

                            if (junior.Id == 0)
                            {                                
                                junior.JuniorKey =
                                    CustomStringHelper.BuildKey(new[]
                                    {
                                        juniorViewModel.Surname,
                                        juniorViewModel.Firstname                                        ,
                                        juniorViewModel.Dob.ToShortDateString()
                                    });
                            }

                            junior.Dob = juniorViewModel.Dob;

                            // check data has not already been saved
                            junior.Member = DuplicateCheck(session, junior.Member,
                                juniorViewModel.MemberKey);

                            // update width new user details
                            MapperHelper.Map(junior.Member, juniorViewModel);
                            MapperHelper.Map(junior.JuniorInfo, juniorViewModel);

                            // create website account
                            if (junior.Member.UserId == 0)
                            {
                                junior.Member.UserId =
                                    CreateAccount(junior.Member,
                                        guardianViewModel.Email, true);
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
                            var fee = new Fee { Season = editMemberViewModel.Season };
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

                            Add(junior.Member.Id);

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

        public void UpdateGuardian(EditMemberViewModel editMemberViewModel)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var guardianList = UpdateGuardians(session, editMemberViewModel.Guardians);
                    var juniorList = UpdateJuniors(session, editMemberViewModel.Juniors, guardianList.First().ContactDetail.Email);

                    foreach (var junior in juniorList)
                    {
                        // save or update junior
                        SetAudit(junior);
                        session.SaveOrUpdate(junior);
                    }

                    foreach (var guardian in guardianList)
                    {
                       
                        foreach (var junior in juniorList)
                        {
                           // check if junior already added to guardian
                            var exists = guardian.Juniors.Any(associatedJunior => associatedJunior.JuniorKey == junior.JuniorKey);
                            if (!exists)
                                guardian.AddJunior(junior);
                        }

                        // add fees for this season
                        AddFee(session, guardian.Juniors);

                        // save or update guardian
                        SetAudit(guardian);
                        session.SaveOrUpdate(guardian);
                    }
                                   
                    transaction.Commit();
                }
            }
        }

        public List<MemberViewModel> GetAll()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var returnList = new List<MemberViewModel>();
            
                var guardianList = session.CreateCriteria(typeof (Guardian))
                    .List<Guardian>().ToList();
                
                var guardians = (from guardian in guardianList
                    let memberViewModel = new MemberViewModel()
                    select MapperHelper.Map(memberViewModel, guardian)).ToList();
           
                var juniorList = session.CreateCriteria(typeof(Junior))
                    .List<Junior>().ToList();

                var juniors = (from junior in juniorList
                    let memberViewModel = new MemberViewModel()
                    select MapperHelper.Map(_configurationServices, memberViewModel, junior)).ToList();

                returnList.AddRange(guardians);
                returnList.AddRange(juniors);

                return returnList;
            }
        }

        public EditMemberViewModel Get(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var adminEditMemberViewModel = new EditMemberViewModel();

                var guardian = session.CreateCriteria(typeof (Guardian))
                    .List<Guardian>().FirstOrDefault(x => x.Member.Id.Equals(id));

                if (guardian!=null)
                {
                    adminEditMemberViewModel.MemberType = (int) MemberType.Guardian;
                    var guardianMemberModel = MapperHelper.Map(new _MemberViewModel(), guardian.Member);
                    MapperHelper.Map(guardianMemberModel, guardian.ContactDetail);
                    MapperHelper.Map(guardianMemberModel, guardian.AddressDetail);

                    guardianMemberModel.Email = guardian.ContactDetail.Email;
                    adminEditMemberViewModel.Guardians.Add(guardianMemberModel);

                    foreach (var junior in guardian.Juniors)
                    {                        
                        var juniorMemberModel = MapperHelper.Map(new _MemberViewModel(), junior.Member);
                        juniorMemberModel.Dob = junior.Dob;
                        adminEditMemberViewModel.Juniors.Add(juniorMemberModel);
                    }
                }
                else
                {
                    var junior = session.CreateCriteria(typeof(Junior))
                    .List<Junior>().FirstOrDefault(x => x.Member.Id.Equals(id));

                    if (junior != null)
                    {
                        adminEditMemberViewModel.MemberType = (int) MemberType.Junior;
                        var juniorMemberModel = MapperHelper.Map(new _MemberViewModel(), junior.Member);
                        adminEditMemberViewModel.Juniors.Add(juniorMemberModel);

                        foreach (var juniorGuardian in junior.Guardians)
                        {
                            var guardianMemberModel = MapperHelper.Map(new _MemberViewModel(), juniorGuardian.Member);
                            adminEditMemberViewModel.Guardians.Add(guardianMemberModel);
                        }
                    }
                }

                //var guardians = (from guardian in guardianList
                //    let memberViewModel = new MemberViewModel()
                //    select MapperHelper.Map(memberViewModel, guardian)).ToList();

             

                //var juniors = (from junior in juniorList
                //    let memberViewModel = new MemberViewModel()
                //    select MapperHelper.Map(_configurationServices, memberViewModel, junior)).ToList();

                //returnList.AddRange(guardians);
                //returnList.AddRange(juniors);

                return adminEditMemberViewModel;
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

        public MemberViewModel GetByKey(string key)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var memberViewModel = new MemberViewModel();
                key = CustomStringHelper.BuildKey(new[] {key});
                var member = session.CreateCriteria(typeof (Member))
                    .List<Member>().FirstOrDefault(x => x.MemberKey.Equals(key));
                return MapperHelper.Map(memberViewModel, member);
            }
        }

        public MemberViewModel GetByUserId(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var memberViewModel = new MemberViewModel();

                var member = session.CreateCriteria(typeof(Member))
                    .List<Member>().FirstOrDefault(x => x.UserId.Equals(id));

                var viewModel = MapperHelper.Map(memberViewModel, member);

                var entityType = checkType(member);

                if (entityType.GetType() == typeof (Guardian))
                {
                    var cakes = 1;
                }

                return viewModel;
            }
        }

        public IUser AuthenticatedUser()
        {
            var user = _authenticationService.GetAuthenticatedUser();
            return _authenticationService.GetAuthenticatedUser();
        }     

        public List<JuniorVettingViewModel> GetNonVetted()
        {

            using (var session = NHibernateHelper.OpenSession())
            {
                var juniorList = session.CreateCriteria(typeof(Junior))
                    .List<Junior>().Where(x => x.IsVetted.Equals(0)).ToList();

                return (from junior in juniorList
                        let juniorVettingViewModel = new JuniorVettingViewModel()
                        select MapperHelper.Map(_configurationServices, juniorVettingViewModel, junior)).ToList();

            }
        }

        public void Activate(int id, JuniorVettingViewModel item)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entity = session.CreateCriteria(typeof(Junior))
                        .List<Junior>().FirstOrDefault(x => x.Id.Equals(id));

                    entity.IsVetted = item.IsVetted;
                    entity.IsActive = item.IsVetted;

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);

                    // activate guardians
                    foreach (var guardian in entity.Guardians)
                    {
                        guardian.IsActive = item.IsVetted;
                        SetAudit(guardian);
                        session.SaveOrUpdate(guardian);                        
                    }

                    transaction.Commit();
                }
            }
        }

        public void Clear()
        {
            ItemsInternal.Clear();
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        private List<int> ItemsInternal
        {
            get
            {
                var items = (List<int>)HttpContext.Session["NewRegistrations"];

                if (items != null) return items;
                items = new List<int>();
                HttpContext.Session["NewRegistrations"] = items;

                return items;
            }
        }

        private List<Guardian> UpdateGuardians(ISession session, List<_MemberViewModel> guardians)
        {
            var rtnList = new List<Guardian>();
            // loop through guardians
            foreach (var guardianViewModel in guardians)
            {
                // get guardian or create a new one
                var guardian = session.CreateCriteria(typeof(Guardian))
                    .List<Guardian>().FirstOrDefault(
                        x => x.Member.Id.Equals(guardianViewModel.MemberId)) ??
                               new Guardian();

                if (guardian.Id == 0)
                {
                    guardian.Init();
                }

                if (string.IsNullOrEmpty(guardianViewModel.MemberKey))
                {
                    guardianViewModel.MemberKey =
                        CustomStringHelper.BuildKey(new[]
                                {
                                    guardianViewModel.Email
                                });
                    guardianViewModel.ContactDetailKey = guardianViewModel.MemberKey;
                    guardian.GuardianKey = guardianViewModel.MemberKey;
                }


                if (string.IsNullOrEmpty(guardianViewModel.AddressDetailKey))
                {
                    guardianViewModel.AddressDetailKey =
                        CustomStringHelper.BuildKey(new[] { guardianViewModel.Address, guardianViewModel.Postcode });
                }

                // check data has not already been saved
                guardian.Member = DuplicateCheck(session, guardian.Member,
                    guardianViewModel.MemberKey);

                guardian.ContactDetail = DuplicateCheck(session, guardian.ContactDetail,
                    guardianViewModel.ContactDetailKey);

                guardian.AddressDetail = DuplicateCheck(session, guardian.AddressDetail,
                    guardianViewModel.AddressDetailKey);

                // update width new user details
                MapperHelper.Map(guardian.Member, guardianViewModel);
                MapperHelper.Map(guardian.ContactDetail, guardianViewModel);
                MapperHelper.Map(guardian.AddressDetail, guardianViewModel);

                // create website account 
                if (guardian.Member.UserId == 0)
                {
                    guardian.Member.UserId =
                        CreateAccount(guardian.Member,
                            guardianViewModel.Email, false);
                }

                // save or update guardian elements
                SetAudit(guardian.Member);
                session.SaveOrUpdate(guardian.Member);
                SetAudit(guardian.ContactDetail);
                session.SaveOrUpdate(guardian.ContactDetail);
                SetAudit(guardian.AddressDetail);
                session.SaveOrUpdate(guardian.AddressDetail);

                // add junior details               
                rtnList.Add(guardian);

            }
            return rtnList;
        }

        private List<Junior> UpdateJuniors(ISession session, List<_MemberViewModel> juniors, string email)
        {
            var rtnList = new List<Junior>();

            foreach (var juniorViewModel in juniors)
            {
                // get junior or create a new one
                var junior = session.CreateCriteria(typeof(Junior))
                    .List<Junior>().FirstOrDefault(
                        x => x.Member.Id.Equals(juniorViewModel.MemberId)) ??
                             new Junior();

                if (junior.Id == 0)
                {
                    junior.Init();
                    junior.Player.Init();
                }

                if (string.IsNullOrEmpty(juniorViewModel.MemberKey))
                {
                    juniorViewModel.MemberKey =
                        CustomStringHelper.BuildKey(new[]
                                    {
                                        juniorViewModel.Surname,
                                        juniorViewModel.Firstname,
                                        juniorViewModel.Dob.ToShortDateString()
                                    });
                }

                junior.JuniorKey = juniorViewModel.MemberKey;

                junior.Dob = juniorViewModel.Dob;

                // check data has not already been saved
                junior.Member = DuplicateCheck(session, junior.Member,
                    juniorViewModel.MemberKey);

                // update width new user details
                if (junior.Id > 0)
                {
                    MapperHelper.UpdateMap(junior.Member, juniorViewModel);
                }
                else
                {
                    MapperHelper.Map(junior.Member, juniorViewModel);
                }
                MapperHelper.Map(junior.JuniorInfo, juniorViewModel);

                // create website account
                if (junior.Member.UserId == 0)
                {
                    junior.Member.UserId =
                        CreateAccount(junior.Member,
                            email, true);
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
                //   var fee = new Fee { Season = registrationViewModel.Season };
                //   SetAudit(fee);
                //   session.SaveOrUpdate(fee);
                //   junior.Player.Fees.Add(fee);

                junior.Player.Number = junior.Member.Id.ToString(CultureInfo.InvariantCulture)
                    .PadLeft(6, '0');

                SetAudit(junior.Player);
                session.SaveOrUpdate(junior.Player);

                rtnList.Add(junior);                             
            }

            return rtnList;
        }

        private void AddFee(ISession session, IList<Junior> juniors)
        {

        }


        private void Add(int memberId)
        {
            ItemsInternal.Add(memberId);
        }

        private object checkType(Member member)
        {
            return new Guardian();
        }        
    }
}
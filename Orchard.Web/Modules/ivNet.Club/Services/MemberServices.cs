
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

        //void CreateGuardian(EditMemberViewModel registrationList);
        void UpdateMember(EditMemberViewModel registrationUpdateList);

        List<RelatedMemberViewModel> GetAll(byte vetted);
        EditMemberViewModel Get(int id);
        List<GuardianViewModel> GetGuardians(int id);
        MemberViewModel GetByKey(string key);
        _MemberViewModel GetFullName(string name);
        MemberViewModel GetByUserId(int id);

       // List<JuniorVettingViewModel> GetNonVetted();
        void Activate(int id, int memberType);
        
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

        public void UpdateMember(EditMemberViewModel editMemberViewModel)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    Clear();

                    bool activeGaurdians;
                    var guardianList = UpdateGuardians(session, editMemberViewModel.Guardians, out activeGaurdians);

                    var juniorList = UpdateJuniors(session, editMemberViewModel.Juniors, activeGaurdians);


                    foreach (var junior in juniorList)
                    {
                        // save or update junior
                        SetAudit(junior);
                        session.SaveOrUpdate(junior);
                        Add(junior.Member.Id);
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

        //public void UpdateGuardian(EditMemberViewModel editMemberViewModel)
        //{
        //    using (var session = NHibernateHelper.OpenSession())
        //    {
        //        using (var transaction = session.BeginTransaction())
        //        {
        //            var guardianList = UpdateGuardians(session, editMemberViewModel.Guardians);
        //            var juniorList = UpdateJuniors(session, editMemberViewModel.Juniors, guardianList.First().ContactDetail.Email);

        //            foreach (var junior in juniorList)
        //            {
        //                // save or update junior
        //                SetAudit(junior);
        //                session.SaveOrUpdate(junior);
        //            }

        //            foreach (var guardian in guardianList)
        //            {
                       
        //                foreach (var junior in juniorList)
        //                {
        //                   // check if junior already added to guardian
        //                    var exists = guardian.Juniors.Any(associatedJunior => associatedJunior.JuniorKey == junior.JuniorKey);
        //                    if (!exists)
        //                        guardian.AddJunior(junior);
        //                }

        //                // add fees for this season
        //                AddFee(session, guardian.Juniors);

        //                // save or update guardian
        //                SetAudit(guardian);
        //                session.SaveOrUpdate(guardian);
        //            }
                                   
        //            transaction.Commit();
        //        }
        //    }
        //}

        public List<RelatedMemberViewModel> GetAll(byte vetted)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var returnList = new List<RelatedMemberViewModel>();

                var guardianList = session.CreateCriteria(typeof (Guardian))
                    .List<Guardian>().Where(x => x.IsVetted.Equals(vetted)).ToList();
                
                var guardians = (from guardian in guardianList
                    let memberViewModel = new RelatedMemberViewModel()
                    select MapperHelper.Map(memberViewModel, guardian)).ToList();

                var juniorList = session.CreateCriteria(typeof(Junior))
                    .List<Junior>().Where(x => x.IsVetted.Equals(vetted)).ToList();

                var juniors = (from junior in juniorList
                               let memberViewModel = new RelatedMemberViewModel()
                    select MapperHelper.Map(memberViewModel, junior)).ToList();

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
                    
                    guardianMemberModel.MemberIsActive = guardian.IsActive;
                    guardianMemberModel.Email = guardian.ContactDetail.Email;
                    
                    adminEditMemberViewModel.Guardians.Add(guardianMemberModel);

                    foreach (var junior in guardian.Juniors)
                    {                        
                        var juniorMemberModel = MapperHelper.Map(new _MemberViewModel(), junior.Member);
                        MapperHelper.Map(juniorMemberModel, junior.JuniorInfo);
                        juniorMemberModel.Dob = junior.Dob;
                        juniorMemberModel.MemberIsActive = junior.IsActive;
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
                        MapperHelper.Map(juniorMemberModel, junior.JuniorInfo);
                        
                        juniorMemberModel.Dob = junior.Dob;
                        juniorMemberModel.MemberIsActive = junior.IsActive;
                        
                        adminEditMemberViewModel.Juniors.Add(juniorMemberModel);

                        foreach (var juniorGuardian in junior.Guardians)
                        {
                            var guardianMemberModel = MapperHelper.Map(new _MemberViewModel(), juniorGuardian.Member);
                            MapperHelper.Map(guardianMemberModel, juniorGuardian.ContactDetail);
                            MapperHelper.Map(guardianMemberModel, juniorGuardian.AddressDetail);
                            guardianMemberModel.MemberIsActive = juniorGuardian.IsActive;
                            adminEditMemberViewModel.Guardians.Add(guardianMemberModel);
                        }
                    }
                }
         
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

        public _MemberViewModel GetFullName(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var memberViewModel = new _MemberViewModel();
                var member = session.CreateCriteria(typeof (Member))
                    .List<Member>().FirstOrDefault(x => (x.Surname + x.Firstname).Equals(name));
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

        //public List<JuniorVettingViewModel> GetNonVetted()
        //{

        //    using (var session = NHibernateHelper.OpenSession())
        //    {
        //        var juniorList = session.CreateCriteria(typeof(Junior))
        //            .List<Junior>().Where(x => x.IsVetted.Equals(0)).ToList();

        //        return (from junior in juniorList
        //                let juniorVettingViewModel = new JuniorVettingViewModel()
        //                select MapperHelper.Map(_configurationServices, juniorVettingViewModel, junior)).ToList();

        //    }
        //}

        public void Activate(int id, int memberType)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    switch (memberType)
                    {
                        case (int) MemberType.Guardian:
                            var guardian = session.CreateCriteria(typeof (Guardian))
                                .List<Guardian>().FirstOrDefault(x => x.Member.Id.Equals(id));

                            if (guardian != null)
                            {
                                ActivateGuardian(guardian);
                                foreach (var guardianJunior in guardian.Juniors)
                                {
                                    ActivateJunior(guardianJunior, guardian.ContactDetail.Email);
                                    SetAudit(guardianJunior);
                                    session.SaveOrUpdate(guardianJunior);
                                }
                             
                                SetAudit(guardian);
                                session.SaveOrUpdate(guardian);
                            }
                            break;
                        case (int) MemberType.Junior:
                            var junior = session.CreateCriteria(typeof (Junior))
                                .List<Junior>().FirstOrDefault(x => x.Member.Id.Equals(id));

                            if (junior != null)
                            {
                                ActivateJunior(junior);
                                foreach (var juniorGuardian in junior.Guardians)
                                {
                                    ActivateGuardian(juniorGuardian);
                                    SetAudit(juniorGuardian);
                                    session.SaveOrUpdate(juniorGuardian);
                                }
                            }
                            break;
                    }
                    //var entity = session.CreateCriteria(typeof(Junior))
                    //    .List<Junior>().FirstOrDefault(x => x.Id.Equals(id));

                    //entity.IsVetted = item.IsVetted;
                    //entity.IsActive = item.IsVetted;

                    //SetAudit(entity);
                    //session.SaveOrUpdate(entity);

                    //// activate guardians
                    //foreach (var guardian in entity.Guardians)
                    //{
                    //    guardian.IsActive = item.IsVetted;
                    //    SetAudit(guardian);
                    //    session.SaveOrUpdate(guardian);                        
                    //}

                    transaction.Commit();
                }
            }
        }
       
        public void Clear()
        {
            ItemsInternal.Clear();
        }

        private void ActivateGuardian(Guardian guardian)
        {
            guardian.IsVetted = 1;
            guardian.IsActive = 1;
            guardian.Member.IsActive = 1;
            guardian.AddressDetail.IsActive = 1;
            guardian.ContactDetail.IsActive = 1;

            // create website account 
            if (guardian.Member.UserId == 0)
            {
                guardian.Member.UserId =
                    CreateAccount(guardian.Member,
                        guardian.ContactDetail.Email, false);

                CreateAcivationEmail(guardian.Member, guardian.Member.UserId, guardian.ContactDetail.Email);
            }          
        }
     
        private void ActivateJunior(Junior junior, string eMail)
        {
            junior.IsVetted = 1;
            junior.IsActive = 1;
            junior.Member.IsActive = 1;
            junior.Player.IsActive = 1;

            // create website account 
            if (junior.Member.UserId == 0)
            {
                junior.Member.UserId =
                    CreateAccount(junior.Member,
                        eMail, true);

                CreateAcivationEmail(junior.Member, junior.Member.UserId, eMail);
            }
        }

        private void CreateAcivationEmail(Member member, int userId, string email)
        {
            throw new NotImplementedException();
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

        private List<Guardian> UpdateGuardians(ISession session, List<_MemberViewModel> guardians, out bool activeGaurdians)
        {
            var rtnList = new List<Guardian>();
            activeGaurdians = false;
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
                }

                guardianViewModel.ContactDetailKey = guardianViewModel.MemberKey;
                guardian.GuardianKey = guardianViewModel.MemberKey;

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
                guardian.IsActive = guardianViewModel.MemberIsActive;               

                // save or update guardian elements
                SetAudit(guardian.Member);
                session.SaveOrUpdate(guardian.Member);
                SetAudit(guardian.ContactDetail);
                session.SaveOrUpdate(guardian.ContactDetail);
                SetAudit(guardian.AddressDetail);
                session.SaveOrUpdate(guardian.AddressDetail);

                // add junior details               
                rtnList.Add(guardian);

                if (!activeGaurdians)
                {
                    activeGaurdians = guardian.IsActive == 1;
                }

            }
            return rtnList;
        }

        private List<Junior> UpdateJuniors(ISession session, List<_MemberViewModel> juniors, bool activeGaurdians)
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
                junior.IsActive = juniorViewModel.MemberIsActive;                

                // save or update junior elements
                SetAudit(junior.Member);
                session.SaveOrUpdate(junior.Member);
                SetAudit(junior.JuniorInfo);
                session.SaveOrUpdate(junior.JuniorInfo);

                MapperHelper.Map(junior.Player.Kit, juniorViewModel);
                SetAudit(junior.Player.Kit);
                session.SaveOrUpdate(junior.Player.Kit);              

                junior.Player.Number = junior.Member.Id.ToString(CultureInfo.InvariantCulture)
                    .PadLeft(6, '0');

                if (!activeGaurdians) junior.IsActive = 0;
                
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
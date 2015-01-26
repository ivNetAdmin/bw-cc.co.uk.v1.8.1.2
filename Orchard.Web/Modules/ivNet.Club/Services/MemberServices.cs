
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using FluentNHibernate.Utils;
using ivNet.Club.Entities;
using ivNet.Club.Enums;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using Newtonsoft.Json;
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
        //void CreateNewMember(NewMemberViewModel viewModel);

        //void CreateGuardian(EditMemberViewModel registrationList);
        void UpdateMember(EditMemberViewModel registrationUpdateList);

        List<MemberViewModel> GetPaginated(int currentPage, string orderBy, bool orderByReverse, int pageItems, string filterBy, string filterByFields);
        List<RelatedMemberViewModel> GetAll(byte vetted);
        EditMemberViewModel Get(int id);
        List<GuardianViewModel> GetGuardians(int id);
        MemberViewModel GetByKey(string key);
        MemberViewModel GetFullName(string name);
        MemberViewModel GetByUserId(int id);
        ContactAdminViewModel GetContactAdminViewModel();
        EditMemberViewModel GetRegisteredUser();

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

        //public void CreateNewMember(NewMemberViewModel viewModel)
        //{
        //    using (var session = NHibernateHelper.OpenSession())
        //    {
        //        using (var transaction = session.BeginTransaction())
        //        {
        //            transaction.Commit();
        //        }
        //    }
        //}

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

        public List<MemberViewModel> GetPaginated(int currentPage, string orderBy, bool orderByReverse, int pageItems,
            string filterBy,
            string filterByFields)
        {

            using (var session = NHibernateHelper.OpenSession())
            {
                var sql = "select ivnetMember.MemberId,ivnetMember.Surname,ivnetMember.Firstname," +
                                   "ivNetJunior.DOB,ivNetJunior.JuniorId," +
                                   "ivNetGuardian.GuardianId," +
                                   "ivnetMember.IsActive " +
                                   "from ivnetMember " +
                                   "left outer join ivNetJunior on ivnetMember.MemberId = ivNetJunior.MemberId " +
                                   "left outer join ivNetGuardian on ivnetMember.MemberId = ivNetGuardian.MemberId";


                var fields = JsonConvert.DeserializeObject<dynamic>(filterByFields);
                              

                var whereClause = new StringBuilder();
                if (fields.Surname != null)
                {
                    whereClause.Append(string.Format("ivnetMember.Surname like '%{0}%' ", fields.Surname));

                }

                if (fields.Firstname != null)
                {
                    whereClause.Append(string.Format("{0}ivnetMember.Firstname like '%{1}%' ",
                        whereClause.Length==0 ? string.Empty : " and ", fields.Firstname));

                }

                if (fields.MemberType != null)
                {
                    const string guardian = "guardian";
                    const string junior = "junior";
                    
                    if (guardian.Contains(((string)fields.MemberType).ToLowerInvariant()))
                    {
                        whereClause.Append(string.Format("{0}ivNetGuardian.GuardianId>0 ",
                            whereClause.Length == 0 ? string.Empty : " and "));
                    }

                    if (junior.Contains(((string)fields.MemberType).ToLowerInvariant()))
                    {
                        whereClause.Append(string.Format("{0}ivNetJunior.JuniorId>0 ",
                            whereClause.Length == 0 ? string.Empty : " and "));
                    }
                }

                if (fields.IsActive != null)
                {
                    if (Extensions.ToLowerInvariantString(fields.IsActive)=="y")
                    {
                        whereClause.Append(string.Format("{0}ivnetMember.IsActive=1 ",
                            whereClause.Length == 0 ? string.Empty : " and "));
                    }
                    else if (Extensions.ToLowerInvariantString(fields.IsActive) == "n")
                    {
                        whereClause.Append(string.Format("{0}ivnetMember.IsActive=0 ",
                            whereClause.Length == 0 ? string.Empty : " and "));
                    }                  
                }
                if (whereClause.Length>0)
                {
                    sql = string.Format("{0} where {1}", sql, whereClause);
                }
           
                var memberContactQuery = session.CreateSQLQuery(sql);
                var members = memberContactQuery.DynamicList();

                var returnList = members.Select(member => new MemberViewModel
                {
                    MemberId = member.MemberID, 
                    Surname = member.Surname, 
                    Firstname = member.Firstname, 
                    MemberIsActive = member.IsActive, 
                    Dob = member.Dob, MemberType = member.GuardianID == null ? "Junior" : "Guardian"
                }).ToList();             

                switch (orderBy)
                {
                    case "IsActive":
                        return orderByReverse ?
                           returnList.OrderByDescending(m => m.MemberIsActive).Skip(pageItems * currentPage).Take(pageItems).ToList() :
                           returnList.OrderBy(m => m.MemberIsActive).Skip(pageItems * currentPage).Take(pageItems).ToList();
                    case "Dob":
                        return orderByReverse ?
                           returnList.OrderByDescending(m => m.Dob).Skip(pageItems * currentPage).Take(pageItems).ToList() :
                           returnList.OrderBy(m => m.Dob).Skip(pageItems * currentPage).Take(pageItems).ToList();
                    case "MemberType":
                        return orderByReverse ?
                           returnList.OrderByDescending(m => m.MemberType).Skip(pageItems * currentPage).Take(pageItems).ToList() :
                           returnList.OrderBy(m => m.MemberType).Skip(pageItems * currentPage).Take(pageItems).ToList();
                    case "Firstname":
                        return orderByReverse ?
                           returnList.OrderByDescending(m => m.Firstname).Skip(pageItems * currentPage).Take(pageItems).ToList() :
                           returnList.OrderBy(m => m.Firstname).Skip(pageItems * currentPage).Take(pageItems).ToList();
                    default:
                        return orderByReverse ? 
                            returnList.OrderByDescending(m => m.Surname).Skip(pageItems * currentPage).Take(pageItems).ToList() : 
                            returnList.OrderBy(m => m.Surname).Skip(pageItems * currentPage).Take(pageItems).ToList();
                }            
            }
        }      

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

        public ContactAdminViewModel GetContactAdminViewModel()
        {
            var contactAdminViewModel = new ContactAdminViewModel();

            using (var session = NHibernateHelper.OpenSession())
            {
                var guardians = session.CreateCriteria(typeof (Guardian))                    
                    .List<Guardian>().Where(g=>g.Member.IsActive.Equals(1));

                var juniorKeylist = new List<string>();

                foreach (var guardian in guardians)
                {
                    var guardianViewModel = new MemberViewModel
                    {
                        MemberType = "Guardian",
                        Surname = guardian.Member.Surname,
                        Firstname = guardian.Member.Firstname,
                        Address = guardian.AddressDetail.Address,
                        Town = guardian.AddressDetail.Town,
                        Postcode = guardian.AddressDetail.Postcode,
                        OtherTelephone = guardian.ContactDetail.OtherTelephone,
                        Mobile = guardian.ContactDetail.Mobile,
                        Email = guardian.ContactDetail.Email
                    };

                    foreach (var junior in guardian.Juniors)
                    {
                        if (juniorKeylist.Count == 0)
                        {
                            juniorKeylist.Add(junior.JuniorKey);
                        }
                        else
                        {
                            if (!juniorKeylist.Contains(junior.JuniorKey))
                            {
                                var juniorViewModel = new JuniorContactViewModel
                                {
                                    Surname = junior.Member.Surname,
                                    Firstname = junior.Member.Firstname,
                                    Dob = junior.Dob.ToShortDateString(),
                                    Notes = junior.JuniorInfo.Notes,
                                    Address = guardian.AddressDetail.Address,
                                    Town = guardian.AddressDetail.Town,
                                    Postcode = guardian.AddressDetail.Postcode,
                                    OtherTelephone = guardian.ContactDetail.OtherTelephone,
                                    Mobile = guardian.ContactDetail.Mobile,
                                    Email = guardian.ContactDetail.Email
                                };

                                contactAdminViewModel.JuniorContacts.Add(juniorViewModel);
                                juniorKeylist.Add(junior.JuniorKey);
                            }
                        }
                        

                       
                    }

                    //contactAdminViewModel.Contacts.Add(guardianViewModel);
                }
            }

            return contactAdminViewModel;
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
                    var guardianMemberModel = MapperHelper.Map(new MemberViewModel(), guardian.Member);
                    MapperHelper.Map(guardianMemberModel, guardian.ContactDetail);
                    MapperHelper.Map(guardianMemberModel, guardian.AddressDetail);
                    
                    guardianMemberModel.MemberIsActive = guardian.IsActive;
                    guardianMemberModel.Email = guardian.ContactDetail.Email;
                    
                    adminEditMemberViewModel.Guardians.Add(guardianMemberModel);

                    foreach (var junior in guardian.Juniors)
                    {                        
                        var juniorMemberModel = MapperHelper.Map(new MemberViewModel(), junior.Member);
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
                        
                        var juniorMemberModel = MapperHelper.Map(new MemberViewModel(), junior.Member);
                        MapperHelper.Map(juniorMemberModel, junior.JuniorInfo);
                        
                        juniorMemberModel.Dob = junior.Dob;
                        juniorMemberModel.MemberIsActive = junior.IsActive;
                        
                        adminEditMemberViewModel.Juniors.Add(juniorMemberModel);

                        foreach (var juniorGuardian in junior.Guardians)
                        {
                            var guardianMemberModel = MapperHelper.Map(new MemberViewModel(), juniorGuardian.Member);
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

        public MemberViewModel GetFullName(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var memberViewModel = new MemberViewModel();
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

        public EditMemberViewModel GetRegisteredUser()
        {
            var user = _authenticationService.GetAuthenticatedUser();

            using (var session = NHibernateHelper.OpenSession())
            {
                var editMemberViewModel = new EditMemberViewModel { MemberType = (int)MemberType.Guardian, AuthenticatedUser = user.UserName};

                var guardian = session.CreateCriteria(typeof(Guardian))
                    .List<Guardian>().FirstOrDefault(x => x.Member.UserId.Equals(user.Id));

                if (guardian != null)
                {

                    var associatedGuardianList = guardian.Juniors[0].Guardians;

                    foreach (var associatedGuardian in associatedGuardianList)
                    {
                        var guardianViewModel = new MemberViewModel();
                        MapperHelper.Map(guardianViewModel, associatedGuardian.Member);
                        MapperHelper.Map(guardianViewModel, associatedGuardian.AddressDetail);
                        MapperHelper.Map(guardianViewModel, associatedGuardian.ContactDetail);
                        guardianViewModel.MemberIsActive = associatedGuardian.IsActive;
                        editMemberViewModel.Guardians.Add(guardianViewModel);                        
                    }
                  
                    foreach (var junior in guardian.Juniors)
                    {
                        var juniorViewModel = new MemberViewModel();
                        MapperHelper.Map(juniorViewModel, junior.Member);
                        MapperHelper.Map(juniorViewModel, junior.JuniorInfo);
                        juniorViewModel.Dob = junior.Dob;
                        juniorViewModel.MemberIsActive = junior.IsActive;
                        editMemberViewModel.Juniors.Add(juniorViewModel);
                    }
                }

                return editMemberViewModel;
            }
        }

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
                                    ActivateJunior(guardianJunior);
                                    SetAudit(guardianJunior);
                                    CreateUserAccount(session, guardianJunior.Member, guardian.ContactDetail.Email, true);
                                    session.SaveOrUpdate(guardianJunior);                                   
                                }

                                SetAudit(guardian);
                                CreateUserAccount(session, guardian.Member, guardian.ContactDetail.Email);
                                session.SaveOrUpdate(guardian);                                
                            }
                            break;
                        case (int) MemberType.Junior:
                            var junior = session.CreateCriteria(typeof (Junior))
                                .List<Junior>().FirstOrDefault(x => x.Member.Id.Equals(id));

                            if (junior != null)
                            {
                                ActivateJunior(junior);
                                SetAudit(junior);
                                session.SaveOrUpdate(junior);
                                CreateUserAccount(session, junior.Member, junior.Guardians.First().ContactDetail.Email, true);

                                foreach (var juniorGuardian in junior.Guardians)
                                {
                                    ActivateGuardian(juniorGuardian);
                                    SetAudit(juniorGuardian);
                                    CreateUserAccount(session, juniorGuardian.Member, juniorGuardian.ContactDetail.Email);
                                    session.SaveOrUpdate(juniorGuardian);                                    
                                }
                            }
                            break;
                    }

                    transaction.Commit();
                }
            }
        }

        public void Clear()
        {
            ItemsInternal.Clear();
        }

        private List<Junior> MemberGirdFilter(List<Junior> juniorList, string filterBy, string filterByFields)
        {
            if (!string.IsNullOrEmpty(filterBy))
            {
                juniorList =
                    juniorList.Where(m => m.Member.Surname.Contains(filterBy) 
                        || m.Member.Firstname.Contains(filterBy))
                        .ToList();
            }
            return juniorList;
        }

        private List<Guardian> MemberGirdFilter(List<Guardian> guardianList, string filterBy, string filterByFields)
        {
            if (!string.IsNullOrEmpty(filterBy))
            {
                guardianList =
                    guardianList.Where(m => m.Member.Surname.Contains(filterBy)
                        || m.Member.Firstname.Contains(filterBy))
                        .ToList();
            }
            return guardianList;
        }

        private void ActivateGuardian(Guardian guardian)
        {
            //guardian.IsVetted = 1;
            //guardian.IsActive = 1;
            //guardian.Member.IsActive = 1;
            //guardian.AddressDetail.IsActive = 1;
            //guardian.ContactDetail.IsActive = 1;

            // for data load only
            guardian.IsVetted = 1;
            guardian.IsActive = guardian.Member.IsActive;
            //guardian.Member.IsActive = 1;
            guardian.AddressDetail.IsActive = guardian.Member.IsActive;
            guardian.ContactDetail.IsActive = guardian.Member.IsActive;
               
        }
     
        private void ActivateJunior(Junior junior)
        {
        //    junior.IsVetted = 1;
        //    junior.IsActive = 1;
        //    junior.Member.IsActive = 1;
        //    junior.Player.IsActive = 1;

            // for data load only
            junior.IsVetted = 1;
            junior.IsActive = junior.Member.IsActive;
            //junior.Member.IsActive = 1;
            junior.Player.IsActive = junior.Member.IsActive;
        }

        private void CreateUserAccount(ISession session, Member member, string email, bool junior = false)
        {
            // create website account 
            if (member.UserId != 0) return;

            string username;
            member.UserId = CreateAccount(member, email, junior, out username);
            CreateAcivationEmail(session, member, username, email);
        }

        private void CreateAcivationEmail(ISession session, Member member, string username, string email)
        {
            var activationEmail = new ActivationEmail
            {
                Email = email,
                Surname = member.Surname,
                Firstname = member.Firstname,
                UserName = username,
                IsActive = 1    
            };

            SetAudit(activationEmail);
            session.SaveOrUpdate(activationEmail);
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

        private List<Guardian> UpdateGuardians(ISession session, List<MemberViewModel> guardians, out bool activeGaurdians)
        {
            var rtnList = new List<Guardian>();
            activeGaurdians = false;
            // loop through guardians
            foreach (var guardianViewModel in guardians)
            {
                // get guardian or create a new one
                var guardian = session.CreateCriteria(typeof(Guardian))
                    .List<Guardian>().FirstOrDefault(
                        x => x.Member.MemberKey.Equals(guardianViewModel.MemberKey)) ??
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
                //check if junior already in list
                var alreadyExists = rtnList.Any(g => g.Member.MemberKey == guardian.Member.MemberKey);
                if (!alreadyExists)
                    rtnList.Add(guardian);         

                if (!activeGaurdians)
                {
                    activeGaurdians = guardian.IsActive == 1;
                }

            }
            return rtnList;
        }

        private List<Junior> UpdateJuniors(ISession session, List<MemberViewModel> juniors, bool activeGaurdians)
        {
            var rtnList = new List<Junior>();

            foreach (var juniorViewModel in juniors)
            {
                // get junior or create a new one
                var junior = session.CreateCriteria(typeof(Junior))
                    .List<Junior>().FirstOrDefault(
                        x => x.Member.MemberKey.Equals(juniorViewModel.MemberKey)) ??
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
                                        juniorViewModel.Dob.GetValueOrDefault().ToShortDateString()
                                    });
                }

                junior.JuniorKey = juniorViewModel.MemberKey;

                junior.Dob = juniorViewModel.Dob.GetValueOrDefault();

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

                junior.Player.Name = string.Format("{0}, {1}", junior.Member.Surname, junior.Member.Firstname);

                if (!activeGaurdians) junior.IsActive = 0;
                
                SetAudit(junior.Player);
                session.SaveOrUpdate(junior.Player);

                //check if junior already in list
                var alreadyExists = rtnList.Any(j => j.Member.MemberKey == junior.Member.MemberKey);
                if (!alreadyExists)
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
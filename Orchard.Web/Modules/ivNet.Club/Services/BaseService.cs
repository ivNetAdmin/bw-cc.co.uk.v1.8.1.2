
using System;
using System.Linq;
using ivNet.Club.Entities;
using ivNet.Club.ViewModel;

using NHibernate;
using Orchard.Data;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;

namespace ivNet.Club.Services
{
    public class BaseService 
    {
        private readonly IMembershipService _membershipService;
        private readonly IRoleService _roleService;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;      
        protected readonly IUser CurrentUser;

        public BaseService
             (
             IMembershipService membershipService,
             IAuthenticationService authenticationService,
             IRoleService roleService,
             IRepository<UserRolesPartRecord> userRolesRepository)
        {
            _membershipService = membershipService;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;
            CurrentUser = authenticationService.GetAuthenticatedUser();
        }

        // guardian check
        protected Guardian DuplicateCheck(ISession session, Guardian guardian, string key)
        {
            var entity = session.CreateCriteria(typeof(Guardian))
               .List<Guardian>().FirstOrDefault(x => x.GuardianKey.Equals(key));
            return entity ?? guardian;
        }

        // junior check
        protected Junior DuplicateCheck(ISession session, Junior junior, string key)
        {
            var entity = session.CreateCriteria(typeof(Junior))
               .List<Junior>().FirstOrDefault(x => x.JuniorGuardianKey.Equals(key));
            return entity ?? junior;
        }

        // member check
        protected ClubMember DuplicateCheck(ISession session, ClubMember clubMember, string key)
        {
            var entity = session.CreateCriteria(typeof(ClubMember))
                .List<ClubMember>().FirstOrDefault(x => x.ClubMemberKey.Equals(key));
            return entity ?? clubMember;
        }

        // contact detail check
        protected ContactDetail DuplicateCheck(ISession session, ContactDetail contactDetail, string key)
        {
            var entity = session.CreateCriteria(typeof(ContactDetail))
                .List<ContactDetail>().FirstOrDefault(x => x.ContactDetailKey.Equals(key));
            return entity ?? contactDetail;
        }      

        protected int CreateAccount(ClubMember clubMember, string email)
        {
            // create user
            var userName = string.Format("{0}.{1}", clubMember.Firstname, clubMember.Surname)
                .ToLowerInvariant().Replace(" ", string.Empty);
            var password = string.Format("{0}{1}9", 
                clubMember.Firstname.ToLowerInvariant(), 
                clubMember.Firstname.ToUpperInvariant())
                .Replace(" ", string.Empty);

            email = email.ToLowerInvariant();

            // check if name already exists - if so append member id eg. tom.jones.456
            var userCheck = _membershipService.GetUser(userName);
            if (userCheck != null)
            {
                userName = string.Format("{0}.{1}", userName, clubMember.Id);
            }

            var user = _membershipService.CreateUser(new CreateUserParams(userName, password, email, null, null, false));

            // assign club member role
            var roleRecord = _roleService.GetRoleByName("ivMember");

            var existingAssociation =
                _userRolesRepository.Get(record => record.UserId == user.Id && record.Role.Id == roleRecord.Id);
            if (existingAssociation == null)
            {
                _userRolesRepository.Create(new UserRolesPartRecord { Role = roleRecord, UserId = user.Id });
            }

            return user.Id;
        }
   
        protected void SetAudit(BaseEntity entity, string merge = null)
        {
            var currentUser = CurrentUser != null ? CurrentUser.UserName : "Non-Authenticated";

            entity.ModifiedBy = currentUser;
            entity.ModifiedDate = DateTime.Now;
//            if (string.IsNullOrEmpty(entity.CreatedBy)) entity.CreatedBy = "Data Load";
//            if (entity.CreateDate == DateTime.MinValue) entity.CreateDate = entity.ModifiedDate;
            if (entity.Id != 0) return;
            entity.IsActive = 1;
            entity.CreatedBy = currentUser;
            entity.CreateDate = DateTime.Now;
        }
    }
}
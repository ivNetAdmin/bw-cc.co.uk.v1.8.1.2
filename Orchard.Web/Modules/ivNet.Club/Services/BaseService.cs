
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
               .List<Junior>().FirstOrDefault(x => x.JuniorKey.Equals(key));
            return entity ?? junior;
        }

        // member check
        protected Member DuplicateCheck(ISession session, Member member, string key)
        {
            var entity = session.CreateCriteria(typeof(Member))
                .List<Member>().FirstOrDefault(x => x.MemberKey.Equals(key));
            return entity ?? member;
        }

        // contact detail check
        protected ContactDetail DuplicateCheck(ISession session, ContactDetail contactDetail, string key)
        {
            var entity = session.CreateCriteria(typeof(ContactDetail))
                .List<ContactDetail>().FirstOrDefault(x => x.ContactDetailKey.Equals(key));
            return entity ?? contactDetail;
        }

        // address detail check
        protected AddressDetail DuplicateCheck(ISession session, AddressDetail addressDetail, string key)
        {
            var entity = session.CreateCriteria(typeof(AddressDetail))
                .List<AddressDetail>().FirstOrDefault(x => x.AddressDetailKey.Equals(key));
            return entity ?? addressDetail;
        }     

        protected int CreateAccount(Member member, string email, bool junior)
        {
            // create user
            var userName = junior
                ? string.Format("{0}.{1}", member.Firstname, member.Surname)
                    .ToLowerInvariant().Replace(" ", string.Empty)
                : email;

            var password = string.Format("{0}{1}1", 
                member.Firstname.ToLowerInvariant(), 
                member.Firstname.ToUpperInvariant())
                .Replace(" ", string.Empty);

            email = email.ToLowerInvariant();

            // check if name already exists - if so append member id eg. tom.jones.456
            var userCheck = _membershipService.GetUser(userName);
            if (userCheck != null)
            {
                userName = string.Format("{0}.{1}", userName, member.Id);
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

            if (junior) return user.Id;
           
            // assign adult role
            roleRecord = _roleService.GetRoleByName("ivMyRegistration");

            existingAssociation =
                _userRolesRepository.Get(record => record.UserId == user.Id && record.Role.Id == roleRecord.Id);
            if (existingAssociation == null)
            {
                _userRolesRepository.Create(new UserRolesPartRecord {Role = roleRecord, UserId = user.Id});
            }
            return user.Id;
        }
   
        protected void SetAudit(BaseEntity entity, string merge = null)
        {
            var currentUser = CurrentUser != null ? CurrentUser.UserName : "Non-Authenticated";

            entity.ModifiedBy = currentUser;
            entity.ModifiedDate = DateTime.Now;

            if (entity.Id != 0) return;
            entity.CreatedBy = currentUser;
            entity.CreateDate = DateTime.Now;
        }
    }
}
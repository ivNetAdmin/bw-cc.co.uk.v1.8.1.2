
using System;
using System.Collections.Generic;
using System.Linq;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using NHibernate.Mapping;
using Orchard;
using Orchard.Data;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;

namespace ivNet.Club.Services
{
    public interface IBwccDataServices : IDependency
    {
        List<MemberViewModel> GetMembers();
    }

    public class BwccDataServices : BaseService, IBwccDataServices
    {
        public BwccDataServices(IMembershipService membershipService, 
            IAuthenticationService authenticationService, 
            IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository) 
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
        }

        public List<MemberViewModel> GetMembers()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var memberViewModel = new MemberViewModel();

                //const string sql = "SELECT [id],[title],[firstname],[surname],[nickname],[email]," +
                //                   "[dob],[hometelephone],[worktelephone],[mobiletelephone],[agegroup]," +
                //                   "[medicalnotes],[medication],[diet],[school],[notactive],[createdby]," +
                //                   "[createdate],[modifiedby],[modifieddate] FROM [bwcc_members]";

                const string sql = "select bwcc_members.*, bwcc_member_contact.* " +
                                   "from bwcc_members " +
                                   "right outer join bwcc_member_contact on bwcc_members.id = bwcc_member_contact.contactid";

                var query = session.CreateSQLQuery(sql);
                var results = query.DynamicList();
               
               var  retList = new List<MemberViewModel>();

                foreach (var result in results)
                {
                    retList.Add(new MemberViewModel
                    {
                        MemberKey = result.contacttype.ToString(),
                        Surname = result.surname, 
                        Firstname = result.firstname,
                        Nickname = result.nickname,
                        ContactDetailKey = result.email,
                        Email = result.email,
                        Mobile = result.mobiletelephone,
                        OtherTelephone = string.Format("{0}{1}", result.hometelephone, string.IsNullOrEmpty(result.worktelephone) ? string.Empty : string.Format(" - {0}", result.worktelephone))

                    });
                }

                return retList;

                //key = CustomStringHelper.BuildKey(new[] { key });


                //var member = session.CreateCriteria(typeof(Member))
                //    .List<Member>().FirstOrDefault(x => x.MemberKey.Equals(key));
                //return MapperHelper.Map(memberViewModel, member);
            }
        }
    }
}
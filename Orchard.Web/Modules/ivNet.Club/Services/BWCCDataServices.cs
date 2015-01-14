
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
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace ivNet.Club.Services
{
    public interface IBwccDataServices : IDependency
    {
        string GetMembers();
    }

    public class BwccDataServices : BaseService, IBwccDataServices
    {
        public BwccDataServices(IMembershipService membershipService, 
            IAuthenticationService authenticationService, 
            IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository) 
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
        }

        public string GetMembers()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                try
                {                 

                    //const string sql = "SELECT [id],[title],[firstname],[surname],[nickname],[email]," +
                    //                   "[dob],[hometelephone],[worktelephone],[mobiletelephone],[agegroup]," +
                    //                   "[medicalnotes],[medication],[diet],[school],[notactive],[createdby]," +
                    //                   "[createdate],[modifiedby],[modifieddate] FROM [bwcc_members]";

                    //const string sql = "select bwcc_members.*, bwcc_member_contact.* " +
                    //                   "from bwcc_members " +
                    //                   "right outer join bwcc_member_contact on bwcc_members.id = bwcc_member_contact.contactid";

                    const string membersSql = "select * from bwcc_members";

                    var membersQuery = session.CreateSQLQuery(membersSql);
                    var members = membersQuery.DynamicList();

                    //var retList = new List<MemberViewModel>();

                    foreach (var member in members)
                    {
                        var editMemberViewModel = new EditMemberViewModel();

                        var memberContactSql = string.Format("select * from bwcc_member_contact where memberid = {0}",
                            member.id);

                        var memberContactQuery = session.CreateSQLQuery(memberContactSql);
                        if (memberContactQuery.List().Count == 0)
                        {
                            // guardian
                        }
                        else
                        {
                            // junior
                            var memberContacts = memberContactQuery.List();
                            foreach (var memberContact in memberContacts)
                            {
                                var contactSql = string.Format("select * from bwcc_member where memberid = {0}",
                               memberContact[2]);

                                var contactQuery = session.CreateSQLQuery(contactSql);
                                var contact = contactQuery.DynamicList();
                            }
                        }

                        

                       

                        //retList.Add(new MemberViewModel
                        //{
                        //    MemberKey = result.contacttype.ToString(),
                        //    Surname = result.surname,
                        //    Firstname = result.firstname,
                        //    Nickname = result.nickname,
                        //    ContactDetailKey = result.email,
                        //    Email = result.email,
                        //    Mobile = result.mobiletelephone,
                        //    OtherTelephone =
                        //        string.Format("{0}{1}", result.hometelephone,
                        //            string.IsNullOrEmpty(result.worktelephone)
                        //                ? string.Empty
                        //                : string.Format(" - {0}", result.worktelephone))

                        //});
                    }

                    return "done";
                }
                catch (Exception ex)
                {
                    return string.Format("{0} {1}", ex.Message,
                        ex.InnerException == null ? string.Empty : string.Format("[{0}]", ex.InnerException.Message));
                }
                //key = CustomStringHelper.BuildKey(new[] { key });


                //var member = session.CreateCriteria(typeof(Member))
                //    .List<Member>().FirstOrDefault(x => x.MemberKey.Equals(key));
                //return MapperHelper.Map(memberViewModel, member);
            }
        }
    }
}
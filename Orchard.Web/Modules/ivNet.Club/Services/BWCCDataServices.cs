﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using NHibernate;
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
        List<EditMemberViewModel> GetMembers();
        List<EditFixtureViewModel> GetFixtures();
    }

    public class BwccDataServices : BaseService, IBwccDataServices
    {
        private const string MembersSql = "SELECT bwcc_members.id " +
                                          ",title " +
                                          ",firstname " +
                                          ",surname " +
                                          ",nickname " +
                                          ",email " +
                                          ",dob " +
                                          ",hometelephone " +
                                          ",worktelephone " +
                                          ",mobiletelephone " +
                                          ",agegroup " +
                                          ",medicalnotes " +
                                          ",medication " +
                                          ",diet " +
                                          ",school " +
                                          ",notactive " +
                                          ",createdby " +
                                          ",createdate " +
                                          ",modifiedby " +
                                          ",modifieddate " +
                                          "FROM bwcc_members";

        private const string FixturesSql = "SELECT bwcc_fixtures.id " +
                                           ",bwcc_fixtures.venueid " +
                                           ",bwcc_fixtures.dateplayed " +
                                           ",bwcc_fixtures.teamid " +
                                           ",bwcc_teams.name as team " +
                                           ",bwcc_fixtures.oppositionid " +
                                           ",bwcc_opposition.name as opposition " +
                                           ",bwcc_fixtures.competitionid " +
                                           ",bwcc_fixtures.resultid " +
                                           ",bwcc_resulttype.name as result" +
                                           ",bwcc_fixtures.result " +
                                           "FROM bwcc_fixtures " +
                                           "INNER JOIN bwcc_teams on bwcc_teams.id = bwcc_fixtures.teamid " +
                                           "INNER JOIN bwcc_opposition on bwcc_opposition.id = bwcc_fixtures.oppositionid " +
                                           "INNER JOIN bwcc_resulttype on bwcc_resulttype.id = bwcc_fixtures.resultid ";

        public BwccDataServices(IMembershipService membershipService,
            IAuthenticationService authenticationService,
            IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository)
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
        }

        public List<EditMemberViewModel> GetMembers()
        {
            using (var session = NHibernateHelper.OpenSession())
            {

                var rtnList = new List<EditMemberViewModel>();

                var members = GetBwccMembers(session);

                foreach (var member in members)
                {
                    var editMemberViewModel = new EditMemberViewModel();

                    var memberContacts = GetBwccContacts(session, (int) member[0]);

                    var memberContactList = memberContacts as IList<dynamic> ?? memberContacts.ToList();

                    if (!memberContactList.Any())
                    {
                        // guardian   
                        if (ValidGuardian(session, (int) member[0]))
                        {
                            editMemberViewModel.Guardians.Add(GetBwccGuardian(session, member));
                            // add juniors
                            var juniors = GetJuniors(session, (int) member[0]);

                            foreach (var junior in juniors)
                            {
                                editMemberViewModel.Juniors.Add(GetBwccJunior(junior));
                            }
                        }
                    }                  

                    rtnList.Add(editMemberViewModel);
                }

                return rtnList;

            }
        }

        public List<EditFixtureViewModel> GetFixtures()
        {
            using (var session = NHibernateHelper.OpenSession())
            {

                var rtnList = new List<EditFixtureViewModel>();

                var fixtures = GetBwccFixtures(session);

                foreach (var fixture in fixtures)
                {
                    var editFixtureViewModel = new EditFixtureViewModel();
                    editFixtureViewModel.LegacyFixtureId = (int)fixture[0];
                    editFixtureViewModel.LegacyVenueId = (int)fixture[1];
                    editFixtureViewModel.Venue = editFixtureViewModel.LegacyVenueId == 1 ? "Home" : "Away";
                    editFixtureViewModel.DatePlayed = DateTime.Parse(fixture[2].ToString());
                    editFixtureViewModel.LegacyTeamId = (int)fixture[3];
                    editFixtureViewModel.Team = fixture[4];

                    rtnList.Add(editFixtureViewModel);
                }

                return rtnList;

            }
        }        

        private bool ValidGuardian(ISession session, int memberId)
        {
            var memberContactSql =
                string.Format(
                    "SELECT id,memberid,contactid,contacttype,emergencycontact " +
                    "FROM bwcc_member_contact " +
                    "WHERE contactid = {0} AND contacttype in (1,2,6)",
                    memberId);

            var memberContactQuery = session.CreateSQLQuery(memberContactSql);
            return memberContactQuery.List<dynamic>().Any();
        }      

        private MemberViewModel GetBwccGuardian(ISession session, dynamic member)
        {
            var guardian = new MemberViewModel
            {
                Email = string.IsNullOrEmpty(member[5]) ? GetUniqueEmail((int) member[0]) : member[5]
            };

            var address = GetBwccAddress(session, (int) member[0]);

            PopulateAddress(guardian, address);

            PopulateContactDetails(guardian, member);

            PopulateMemberDetails(guardian, member);          

            return guardian;
        }

        private MemberViewModel GetBwccJunior(dynamic member)
        {
            var junior = new MemberViewModel();                     

            PopulateMemberDetails(junior, member);
            junior.Dob = DateTime.Parse(member[6].ToString());
            junior.MemberKey = CustomStringHelper.BuildKey(new[] { junior.Surname, junior.Firstname, junior.Dob.GetValueOrDefault().ToShortDateString() });
            
            junior.School = member[14];
            junior.Notes = string.Format("{0} {1} {2}", member[11], member[12], member[13]);
        
            return junior;

        }
        
        private string GetUniqueEmail(int memberId)
        {
            return string.Format("unknown{0}@bwcc.co.uk", memberId);
        }

        private void PopulateMemberDetails(MemberViewModel member, dynamic memberDetail)
        {
            // populate member
            member.Surname = memberDetail[3];
            member.Firstname = memberDetail[2];
            member.Nickname = memberDetail[4];
            member.MemberIsActive = Convert.ToByte(memberDetail[15]) == 0 ? (byte) 1 : (byte) 0;
            member.MemberKey = CustomStringHelper.BuildKey(new[] {member.Email});
        }

        private void PopulateContactDetails(MemberViewModel guardian, dynamic member)
        {
            // populate contact                     
            guardian.Mobile = member[9];
            guardian.OtherTelephone = string.Format("{0}{1}", member[7],
                string.IsNullOrEmpty(member[8])
                    ? string.Empty
                    : string.IsNullOrEmpty(member[7]) ? member[8] : string.Format(", {0}", member[8]));
            guardian.ContactDetailKey = CustomStringHelper.BuildKey(new[] {guardian.Email});

        }

        private void PopulateAddress(MemberViewModel guardian, dynamic address)
        {
            // populate address 
            if (address == null)
            {
                guardian.Address = "Unknown";
                guardian.Postcode = "Unknown";
                guardian.AddressDetailKey =
                    CustomStringHelper.BuildKey(new[] { guardian.Address, guardian.Postcode });
            }
            else
            {
                guardian.Address = string.Format("{0}{1}", address[1],
                    string.IsNullOrEmpty(address[2]) ? string.Empty : string.Format(", {0}", address[2]));
                guardian.Postcode = address[5];
                guardian.Town = address[3];
                guardian.AddressDetailKey =
                    CustomStringHelper.BuildKey(new[] {guardian.Address, guardian.Postcode});
            }
        }

        private dynamic GetBwccAddress(ISession session, int memberId)
        {
            // get address
            var memberAddreessSql =
                string.Format(
                    "SELECT id,memberid,addressid FROM bwcc_member_address WHERE memberid = {0} order by addressid",
                    memberId);

            var addressMemberQuery = session.CreateSQLQuery(memberAddreessSql);
            var addressMember = addressMemberQuery.List<dynamic>().LastOrDefault();

            if (addressMember == null) return null;

            var addreessSql =
                string.Format(
                    "SELECT id,address1,address2,towncity,county,postcode FROM bwcc_addresses WHERE id = {0}",
                    (int) addressMember[2]);

            var addressQuery = session.CreateSQLQuery(addreessSql);
            return addressQuery.List<dynamic>().FirstOrDefault();
        }

        private static IEnumerable<dynamic> GetBwccContacts(ISession session, int memberId)
        {
            var memberContactSql =
                string.Format(
                    "SELECT id,memberid,contactid,contacttype,emergencycontact " +
                    "FROM bwcc_member_contact " +
                    "WHERE memberid = {0}",
                    memberId);

            var memberContactQuery = session.CreateSQLQuery(memberContactSql);
            return memberContactQuery.List<dynamic>();
        }

        private IEnumerable<dynamic> GetJuniors(ISession session, int guardianId)
        {
            var juniorsSql =
                string.Format(
                    "{0} INNER JOIN bwcc_member_contact on bwcc_members.id = bwcc_member_contact.memberid " +
                    "and bwcc_member_contact.contactid = {1}", MembersSql, guardianId);

            var juniorsQuery = session.CreateSQLQuery(juniorsSql);
            return juniorsQuery.List<dynamic>();
        }


        private static IEnumerable<dynamic> GetBwccMembers(ISession session)
        {
            var membersQuery = session.CreateSQLQuery(MembersSql);
            return membersQuery.List<dynamic>();
        }

        private static IEnumerable<dynamic> GetBwccFixtures(ISession session)
        {
            var fixturesQuery = session.CreateSQLQuery(FixturesSql);
            return fixturesQuery.List<dynamic>();
        }        
    }
}
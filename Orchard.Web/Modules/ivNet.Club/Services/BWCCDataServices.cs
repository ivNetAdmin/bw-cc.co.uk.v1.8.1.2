
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        List<BWCCLoadFixtureViewModel> GetFixtures();
        List<PlayerStatViewModel> GetFixtureStats(int fixtureId, int legacyFixtureId);
        List<MatchReport> GetMatchReports();
        void SaveMatchReport(MatchReport matchReport);
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
                                           ",bwcc_fixtures.resultid as resulttypeid " +
                                           ",bwcc_resulttype.name as resulttype" +
                                           ",bwcc_fixtures.result " +
                                           "FROM bwcc_fixtures " +
                                           "INNER JOIN bwcc_teams on bwcc_teams.id = bwcc_fixtures.teamid " +
                                           "INNER JOIN bwcc_opposition on bwcc_opposition.id = bwcc_fixtures.oppositionid " +
                                           "LEFT OUTER JOIN bwcc_resulttype on bwcc_resulttype.id = bwcc_fixtures.resultid ";

        private const string FixtureStatsSql = "SELECT bwcc_fixturestats.id " +
                                               ",bwcc_fixtures.teamid " +
                                               ",bwcc_teams.name as team " +
                                               ",fixtureid " +
                                               ",battingposition " +
                                               ",memberid " +
                                               ",runsscored " +
                                               ",howoutid " +
                                               ",bwcc_howout.name as howout " +
                                               ",oversbowled " +
                                               ",maidenovers " +
                                               ",runsconceeded " +
                                               ",wicketstaken " +
                                               ",catches " +
                                               ",stumpings " +
                                               ",locked " +
                                               ",createdby " +
                                               ",createdate " +
                                               ",modifiedby " +
                                               ",modifieddate " +
                                               "FROM bwcc_fixturestats " +
                                               "INNER JOIN bwcc_fixtures on bwcc_fixtures.id = bwcc_fixturestats.fixtureId " +
                                               "INNER JOIN bwcc_teams on bwcc_teams.id = bwcc_fixtures.teamid "+
                                               "LEFT OUTER JOIN bwcc_howout on bwcc_howout.id = bwcc_fixturestats.howoutid";

        private const string MatchReportSql = "SELECT bwcc_fixtures.[id] " +
                                              ",bwcc_fixtures.[teamid] " +
                                              ",bwcc_fixtures.[dateplayed] " +
                                              ",bwcc_teams.name as team " +
                                              ",bwcc_matchreports.[createdby] " +
                                              ",bwcc_matchreports.[createdate] " +
                                              ",bwcc_matchreports.[modifiedby] " +
                                              ",bwcc_matchreports.[modifieddate] " +
                                              ",bwcc_matchreports.[report] " +
                                              "FROM [bwcc_fixtures] " +
                                              "INNER JOIN bwcc_teams on bwcc_teams.id = bwcc_fixtures.teamid " +
                                              "INNER JOIN [bwcc_matchreports] ON bwcc_matchreports.fixtureid=bwcc_fixtures.Id";


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
                    // if guardian they will have a contactId in bwcc_member_contact 
                    // equal to their memberId                  
                    if (IsGuardian(session, (int) member[0]))
                    {                        
                        var editMemberViewModel = new EditMemberViewModel();
                        editMemberViewModel.Guardians.Add(GetBwccSeniorGuardian(session, member));
                        // add juniors
                        var juniors = GetJuniors(session, (int) member[0]);

                        foreach (var junior in juniors)
                        {
                            editMemberViewModel.Juniors.Add(GetBwccJunior(junior));
                        }
                        rtnList.Add(editMemberViewModel);
                    }

                    // if senior they will have a memberId in bwcc_fixturestats 
                    // equal to their memberId
                    if (IsSenior(session, (int)member[0]))
                    {
                        var editMemberViewModel = new EditMemberViewModel();
                        editMemberViewModel.Seniors.Add(GetBwccSeniorGuardian(session, member));
                        rtnList.Add(editMemberViewModel);
                    }

                }

                return rtnList;

            }
        }
       
        public List<BWCCLoadFixtureViewModel> GetFixtures()
        {
            using (var session = NHibernateHelper.OpenSession())
            {

                var rtnList = new List<BWCCLoadFixtureViewModel>();

                var fixtures = GetBwccFixtures(session);
                var fixtureTypes = new [] {"Unknown", "Friendly", "League", "Cup", "6-a-side"};
                foreach (var fixture in fixtures)
                {
                    var editFixtureViewModel = new BWCCLoadFixtureViewModel
                    {
                        LegacyFixtureId = (int) fixture[0],
                        LegacyVenueId = (int) fixture[1],
                        DatePlayed = DateTime.Parse(fixture[2].ToString()),
                        LegacyTeamId = (int) fixture[3],
                        Team = fixture[4],
                        OppisitionId = (int) fixture[5],
                        Oppisition = fixture[6],
                        LegacyFixtureTypeId = (int) fixture[7],
                        ResultTypeId = (int) fixture[8],
                        ResultType = fixture[9],
                        Score = fixture[10]
                    };

                    editFixtureViewModel.Venue = editFixtureViewModel.LegacyVenueId == 1 ? "Home" : "Away";
                    editFixtureViewModel.FixtureType = fixtureTypes[editFixtureViewModel.LegacyFixtureTypeId];

                    rtnList.Add(editFixtureViewModel);
                }

                return rtnList;
            }
        }

        public List<PlayerStatViewModel> GetFixtureStats(int fixtureId, int legacyFixtureId)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var rtnList = new List<PlayerStatViewModel>();
                var fixtureStats = GetBwccFixtureStats(session, legacyFixtureId);
              
                foreach (var fixtureStat in fixtureStats)
                {

                    var player = GetPlayerByLegacyMemberId(session,(int) fixtureStat[5]);

                    var playerStatViewModel = new PlayerStatViewModel
                    {
                        FixtureId = fixtureId,
                        PlayerName = player.Name,
                        PlayerNumber = player.Number,
                        Catches = (int) fixtureStat[13],
                        Maidens = (int) fixtureStat[10],
                        HowOutId = fixtureStat[7],
                        HowOut = fixtureStat[8],
                        Overs = (int) fixtureStat[9],
                        Runs = (int) fixtureStat[6],
                        RunsConceeded = (int) fixtureStat[11],
                        Wickets = (int) fixtureStat[12],
                        Stumpings = (int) fixtureStat[14],
                        BattingPosition = (int) fixtureStat[4]
                    };

                    rtnList.Add(playerStatViewModel);
                }

                return rtnList;
            }            
        }

        public List<MatchReport> GetMatchReports()
        {
            var rtnList = new List<MatchReport>();

            using (var session = NHibernateHelper.OpenSession())
            {
                var matchReports = GetBwccMatchReports(session);

                foreach (var legacyMatchReport in matchReports)
                {
                   var fixture = GetFixturIdByTeamDate(session, legacyMatchReport[3], DateTime.Parse(legacyMatchReport[2].ToString()));
               
                    var matchReport = new MatchReport();
                    matchReport.Init();
                    matchReport.Report = legacyMatchReport[8];
                    matchReport.CreateDate = DateTime.Parse(legacyMatchReport[5].ToString());
                    matchReport.CreatedBy = legacyMatchReport[4];
                    matchReport.Fixture = fixture;
                    matchReport.IsActive = 1;
                    matchReport.ModifiedBy = legacyMatchReport[6];
                    matchReport.ModifiedDate = DateTime.Parse(legacyMatchReport[7].ToString());
                
                    if (string.IsNullOrEmpty(matchReport.CreatedBy)) matchReport.CreatedBy = "ivNetAdmin";
                    if (string.IsNullOrEmpty(matchReport.ModifiedBy)) matchReport.ModifiedBy = "ivNetAdmin";

                    rtnList.Add(matchReport);
                }
            }
            return rtnList;
        }

        public void SaveMatchReport(MatchReport matchReport)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    if (matchReport.Report.Length > 4000) matchReport.Report = matchReport.Report.Substring(0, 4000);
                    session.SaveOrUpdate(matchReport);
                    transaction.Commit();
                }
            }           
        }

        private Fixture GetFixturIdByTeamDate(ISession session, string team, DateTime datePlayed)
        {
            return session.CreateCriteria(typeof(Fixture))
                  .List<Fixture>().FirstOrDefault(x => x.Date.Equals(datePlayed) && x.Team.Name.Equals(team));
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

        private MemberViewModel GetBwccSeniorGuardian(ISession session, dynamic member)
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
            junior.MemberKey = CustomStringHelper.BuildKey(new[] { junior.Surname, junior.Firstname
            //    , junior.Dob.GetValueOrDefault().ToShortDateString() 
            });
            
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
            //member.MemberKey = CustomStringHelper.BuildKey(new[] {member.Email});
            member.MemberKey = CustomStringHelper.BuildKey(new[] { member.Surname, member.Firstname });
            member.LegacyId = (int)memberDetail[0];
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
            //var membersQuery = session.CreateSQLQuery(MembersSql + " WHERE agegroup = 'S'");
            var membersQuery = session.CreateSQLQuery(MembersSql);
            return membersQuery.List<dynamic>();
        }

        private static IEnumerable<dynamic> GetBwccFixtures(ISession session)
        {
            var fixturesQuery = session.CreateSQLQuery(FixturesSql);
            return fixturesQuery.List<dynamic>();
        }

        private static IEnumerable<dynamic> GetBwccMatchReports(ISession session)
        {
            var matchReportsQuery = session.CreateSQLQuery(MatchReportSql);
            return matchReportsQuery.List<dynamic>();
        }
        
        private static IEnumerable<dynamic> GetBwccFixtureStats(ISession session, int legacyFixtureId)
        {
            var statsSql =
              string.Format(
                  "{0} WHERE bwcc_fixturestats.fixtureId = {1}", FixtureStatsSql, legacyFixtureId);

            var fixtureStatsQuery = session.CreateSQLQuery(statsSql);
            return fixtureStatsQuery.List<dynamic>();
        }

        private static Member GetMemberByLegacyId(ISession session, int memberId)
        {
           return session.CreateCriteria(typeof(Member))
                  .List<Member>().FirstOrDefault(x => x.LegacyId.Equals(memberId));
        }

        private static Player GetPlayerByNumber(ISession session, string playerNo)
        {
            return session.CreateCriteria(typeof(Player))
                   .List<Player>().FirstOrDefault(x => x.Number.Equals(playerNo));
        }

        private Player GetPlayerByLegacyMemberId(ISession session, int memberLegacyId)
        {
            var member = GetBwccMember(session, memberLegacyId);

            var memberKey = CustomStringHelper.BuildKey(new[] { (string)member[3], (string)member[2] });

            var senior = session.CreateCriteria(typeof(Senior))
                  .List<Senior>().FirstOrDefault(x => x.SeniorKey.Equals(memberKey));

            if (senior != null) return senior.Player;

            var junior = session.CreateCriteria(typeof(Junior))
                 .List<Junior>().FirstOrDefault(x => x.JuniorKey.Equals(memberKey));

            return junior != null ? junior.Player : null;
        }

        private bool IsGuardian(ISession session, int memberId)
        {
            var memberContactSql =
                string.Format(
                    "SELECT id " +
                    "FROM bwcc_member_contact " +
                    "WHERE contactid = {0} AND contacttype in (1,2,6)",
                    memberId);

            var memberContactQuery = session.CreateSQLQuery(memberContactSql);
            return memberContactQuery.List<dynamic>().Any();
        }

        private bool IsJunior(ISession session, int memberId)
        {
            var memberContactSql =
               string.Format(
                   "SELECT id " +
                   "FROM bwcc_member_contact " +
                   "WHERE memberId = {0}",
                   memberId);

            var memberContactQuery = session.CreateSQLQuery(memberContactSql);
            return memberContactQuery.List<dynamic>().Any();
        }

        private bool IsSenior(ISession session, int memberId)
        {
            var memberContactSql =
             string.Format(
                 "SELECT id " +
                 "FROM bwcc_fixturestats " +
                 "WHERE memberId = {0}",
                 memberId);

            var memberContactQuery = session.CreateSQLQuery(memberContactSql);
            return memberContactQuery.List<dynamic>().Any();
        }

        private static dynamic GetBwccMember(ISession session, int memberId)
        {
            var membersQuery = session.CreateSQLQuery(MembersSql + " WHERE bwcc_members.id = '" + memberId + "'");
            //var membersQuery = session.CreateSQLQuery(MembersSql);
            return membersQuery.List<dynamic>().FirstOrDefault();
        }
    }
}
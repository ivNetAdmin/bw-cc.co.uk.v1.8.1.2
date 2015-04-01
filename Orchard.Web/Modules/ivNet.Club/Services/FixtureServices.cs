using System;
using System.Collections.Generic;
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
using Orchard;
using Orchard.Data;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;

namespace ivNet.Club.Services
{
    public interface IFixtureServices : IDependency
    {
        List<FixtureViewModel> GetAll();

        List<FixtureViewModel> GetPaginated(int currentPage, string orderBy, bool orderByReverse, int pageItems,
            string filterBy, string filterByFields, out int totalCount);
        AdminFixtureViewModel GetAdminFixtureViewModel();
        AdminFixtureViewModel GetAdminFixtureViewModel(int fixtureId);
        AdminFixtureViewModel GetAdminFixtureViewModel(int currentPage, string orderBy, bool orderByReverse, int pageItems, string filterBy, string filterByFields);
        
        void SaveFixture(FixtureViewModel item);
        void SaveTeamSelection(int fixtureId, AdminTeamSelectionViewModel teamSelectionViewModel);
        void SaveFixtureStats(List<PlayerStatViewModel> stats);
        void SaveMatchReport(int id, AdminFixtureReportViewModel matchReport);

        TeamSelectionAdminViewModel GetTeamSelectionAdminViewModel(int id);
        FixtureListViewModel GetFixtureListViewModel();

        AdminFixtureStatViewModel GetAdminFixtureStatViewModel(int fixtureId);

        AdminFixtureReportViewModel GetAdminFixtureReportViewModel(int fixtureId);

        FixtureViewModel GetFixtureViewModel(int id);
    }

    public class FixtureServices : BaseService, IFixtureServices
    {
        private readonly IConfigurationServices _configurationServices;

        public FixtureServices(IMembershipService membershipService, 
            IAuthenticationService authenticationService, 
            IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository,
            IConfigurationServices configurationServices) 
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
            _configurationServices = configurationServices;
        }

        public List<FixtureViewModel> GetPaginated(int currentPage, string orderBy, bool orderByReverse, int pageItems,
            string filterBy,
            string filterByFields, out int totalCount)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var sql = "select ivNetFixture.FixtureId,ivNetFixture.Date,ivNetFixture.Score, " +
                          "ivNetFixture.HomeAway,ivNetFixture.IsActive,  " +
                          "ivNetTeam.Name AS Team, ivNetOpponent.Name AS Opponent,  " +
                          "ivNetFixtureType.Name AS Fixture, ivNetResultType.Name AS Result " +
                          "from ivNetFixture  " +
                          "left outer join ivNetTeam on ivNetFixture.TeamId = ivNetTeam.TeamId  " +
                          "left outer join ivNetOpponent on ivNetFixture.OpponentID = ivNetOpponent.OpponentID " +
                          "left outer join ivNetFixtureType on ivNetFixture.FixtureTypeID = ivNetFixtureType.FixtureTypeID " +
                          "left outer join ivNetResultType on ivNetFixture.ResultTypeID = ivNetResultType.ResultTypeID";                          

                var fields = JsonConvert.DeserializeObject<dynamic>(filterByFields);

                var whereClause = new StringBuilder();
                if (fields.Team != null)
                {
                    whereClause.Append(string.Format("ivNetTeam.Name like '%{0}%' ", fields.Team));
                }
                if (fields.Opponent != null)
                {                 
                    whereClause.Append(string.Format("{0}ivNetOpponent.Name like '%{1}%' ",
                        whereClause.Length == 0 ? string.Empty : " and ", fields.Opponent));
                }
                if (fields.HomeAway != null)
                {
                    whereClause.Append(string.Format("{0}ivNetFixture.HomeAway like '%{1}%' ",
                       whereClause.Length == 0 ? string.Empty : " and ", fields.HomeAway));                    
                }
                if (fields.FixtureType != null)
                {
                    whereClause.Append(string.Format("{0}ivNetFixtureType.Name like '%{1}%' ",
                        whereClause.Length == 0 ? string.Empty : " and ", fields.FixtureType));                                     
                }
                if (fields.Score != null)
                {
                    whereClause.Append(string.Format("{0}ivNetFixture.Score like '%{1}%' ",
                        whereClause.Length == 0 ? string.Empty : " and ", fields.Score));
                }
                if (fields.Date != null)
                {
                    whereClause.Append(string.Format("{0}ivNetFixture.Date like '%{1}%' ",
                        whereClause.Length == 0 ? string.Empty : " and ", fields.Date));
                }
                  //if (fields.IsActive != null)
                //{
                //    if (Extensions.ToLowerInvariantString(fields.IsActive) == "y")
                //    {
                //        whereClause.Append(string.Format("{0}ivnetMember.IsActive=1 ",
                //            whereClause.Length == 0 ? string.Empty : " and "));
                //    }
                //    else if (Extensions.ToLowerInvariantString(fields.IsActive) == "n")
                //    {
                //        whereClause.Append(string.Format("{0}ivnetMember.IsActive=0 ",
                //            whereClause.Length == 0 ? string.Empty : " and "));
                //    }
                //}


                if (whereClause.Length > 0)
                {
                    sql = string.Format("{0} where {1}", sql, whereClause);
                }

                var fixtureQuery = session.CreateSQLQuery(sql);
                var fixtures = fixtureQuery.DynamicList();

                var returnList = fixtures.Select(fixture => new FixtureViewModel
                {
                    Id = fixture.FixtureID,
                    Date = fixture.Date,
                    HomeAway = fixture.HomeAway,
                    Team = fixture.Team,
                    Opponent = fixture.Opponent,
                    FixtureType = fixture.Fixture,
                    Score = fixture.Score
                }).ToList();

                List<FixtureViewModel> rtnList;

                totalCount = returnList.Count;

                switch (orderBy)
                {
                //    case "IsActive":
                //        rtnList = orderByReverse
                //            ? returnList.OrderByDescending(m => m.MemberIsActive)
                //                .Skip(pageItems*currentPage)
                //                .Take(pageItems)
                //                .ToList()
                //            : returnList.OrderBy(m => m.MemberIsActive)
                //                .Skip(pageItems*currentPage)
                //                .Take(pageItems)
                //                .ToList();
                //        break;
                    case "Date":
                        rtnList = orderByReverse
                            ? returnList.OrderByDescending(m => m.Date)
                                .Skip(pageItems*currentPage)
                                .Take(pageItems)
                                .ToList()
                            : returnList.OrderBy(m => m.Date).Skip(pageItems * currentPage).Take(pageItems).ToList();
                        break;
                    case "Team":
                        rtnList = orderByReverse
                            ? returnList.OrderByDescending(m => m.Team)
                                .Skip(pageItems * currentPage)
                                .Take(pageItems)
                                .ToList()
                            : returnList.OrderBy(m => m.Team).Skip(pageItems * currentPage).Take(pageItems).ToList();
                        break;
                    case "Opponent":
                        rtnList = orderByReverse
                            ? returnList.OrderByDescending(m => m.Opponent)
                                .Skip(pageItems * currentPage)
                                .Take(pageItems)
                                .ToList()
                            : returnList.OrderBy(m => m.Opponent).Skip(pageItems * currentPage).Take(pageItems).ToList();
                        break;
                    case "HomeAway":
                        rtnList = orderByReverse
                            ? returnList.OrderByDescending(m => m.HomeAway)
                                .Skip(pageItems * currentPage)
                                .Take(pageItems)
                                .ToList()
                            : returnList.OrderBy(m => m.HomeAway).Skip(pageItems * currentPage).Take(pageItems).ToList();
                        break;
                    case "Score":
                        rtnList = orderByReverse
                            ? returnList.OrderByDescending(m => m.Score)
                                .Skip(pageItems * currentPage)
                                .Take(pageItems)
                                .ToList()
                            : returnList.OrderBy(m => m.Score).Skip(pageItems * currentPage).Take(pageItems).ToList();
                        break;
                    default:
                        rtnList = returnList.OrderByDescending(m => m.Date)
                            .Skip(pageItems*currentPage)
                            .Take(pageItems)
                            .ToList();
                        break;
                }

                //foreach (var member in rtnList)
                //{
                //    member.AgeGroup = member.Dob == null
                //        ? ""
                //        : string.Format("U{0}", _configurationServices.GetJuniorYear(member.Dob.GetValueOrDefault()));
                //}

               // return returnList;
               return rtnList;
            }
        }

        public List<FixtureViewModel> GetAll()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var fixtureList = session.CreateCriteria(typeof(Fixture))
                    .List<Fixture>();

                return fixtureList.Select(fixture => MapperHelper.Map(new FixtureViewModel(), fixture)).ToList();                
            }
        }       

        public void SaveFixture(FixtureViewModel item)
        {
            var fixtureKey = CustomStringHelper.BuildKey(new[] { item.Team, item.Date.GetValueOrDefault().ToShortDateString() });
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entity = session.CreateCriteria(typeof (Fixture))
                        .List<Fixture>().FirstOrDefault(x => x.FixtureKey.Equals(fixtureKey)) ?? new Fixture();

                    var team = session.CreateCriteria(typeof(Team))
                       .List<Team>().FirstOrDefault(x => x.Name.Equals(item.Team));

                    var opponent = session.CreateCriteria(typeof (Opponent))
                        .List<Opponent>().FirstOrDefault(x => x.Name.Equals(item.Opponent));

                    var location = session.CreateCriteria(typeof (Location))
                        .List<Location>().FirstOrDefault(x => x.Postcode.Equals(item.Location));

                    var fixtureType = session.CreateCriteria(typeof (FixtureType))
                        .List<FixtureType>().FirstOrDefault(x => x.Name.Equals(item.FixtureType));

                    var resultType = session.CreateCriteria(typeof (ResultType))
                        .List<ResultType>().FirstOrDefault(x => x.Name.Equals(item.ResultType));

                    entity.Date = item.Date.GetValueOrDefault();
                    entity.FixtureKey = CustomStringHelper.BuildKey(new[] {team.Name, entity.Date.ToShortDateString()});
                    entity.Score = item.Score;

                    entity.FixtureType = fixtureType;
                    entity.ResultType = resultType;
                    entity.HomeAway = item.HomeAway;
                    entity.Location = location;
                    entity.Opponent = opponent;
                    entity.Team = team;

                    entity.IsActive = 1;

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);

                    transaction.Commit();

                    item.Id = entity.Id;
                }
            }       
        }

        public void SaveMatchReport(int id, AdminFixtureReportViewModel viewModel)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var fixture = session.CreateCriteria(typeof(Fixture))
                       .List<Fixture>().FirstOrDefault(x => x.Id.Equals(id));

                    if (fixture != null)
                    {
                        if (fixture.MatchReport == null)
                        {
                            fixture.MatchReport=new MatchReport();
                        }

                        fixture.MatchReport.Report = viewModel.MatchReport;

                        fixture.MatchReport.IsActive = 1;
                        SetAudit(fixture.MatchReport);
                        session.SaveOrUpdate(fixture.MatchReport);

                        var resultType = session.CreateCriteria(typeof(ResultType))
                       .List<ResultType>().FirstOrDefault(x => x.Id.Equals(viewModel.ResultType));

                        fixture.Score = viewModel.FixtureScore;
                        fixture.ResultType = resultType;

                        SetAudit(fixture);
                        session.SaveOrUpdate(fixture);
                    }

                    transaction.Commit();
                }
            }
        }

        public void SaveTeamSelection(int fixtureId, AdminTeamSelectionViewModel teamSelectionViewModel)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var teamSelection = session.CreateCriteria(typeof (TeamSelection))
                        .List<TeamSelection>().FirstOrDefault(x => x.Fixture.Id.Equals(fixtureId)) ??
                                 new TeamSelection();
                                      
                    if (teamSelection.Players == null)
                    {
                        teamSelection.Init();
                        teamSelection.IsActive = 1;
                    }
                    else
                    {
                        var playerNumbers = teamSelection.Players.Select(player => player.Number).ToList();

                        foreach (var playerNumber in playerNumbers)
                        {
                            var player = GetPlayer(session, playerNumber);
                            teamSelection.RemovePlayer(player);
                        }
                    }
                   
                    SetAudit(teamSelection);                    

                    teamSelection.Fixture = session.CreateCriteria(typeof(Fixture))
                       .List<Fixture>().FirstOrDefault(x => x.Id.Equals(fixtureId));

                    foreach (var playerViewModel in teamSelectionViewModel.TeamSelection)
                    {
                        if (!string.IsNullOrEmpty(playerViewModel.Name))
                        {
                            teamSelection.AddPlayer(
                                session.CreateCriteria(typeof (Player))
                                    .List<Player>().FirstOrDefault(x => x.Number.Equals(playerViewModel.PlayerNumber))
                                );
                        }
                    }

                    session.SaveOrUpdate(teamSelection);
                    if (teamSelection.Fixture != null) teamSelection.Fixture.TeamSelection = teamSelection;
                    session.SaveOrUpdate(teamSelection.Fixture);
                    var fixtutreId = teamSelection.Fixture == null ? 0 : teamSelection.Fixture.Id;

                    // create player stats - get existing ones or create new ones, delete old ones then save new ones
                    var playerStatList = 
                        teamSelection.Players.Select(player => GetPlayerStat(session, fixtutreId, player.Number)).ToList();

                    // delete any existing stats
                    var queryString = string.Format("delete {0} where Fixture.Id = :id", typeof(PlayerStat));
                    session.CreateQuery(queryString)
                           .SetParameter("id", fixtureId)
                           .ExecuteUpdate();

                    // add new stats
                    foreach (var playerStat in playerStatList)
                    {                      

                        SetAudit(playerStat.CricketStat);
                        playerStat.CricketStat.IsActive = 1;
                        session.SaveOrUpdate(playerStat.CricketStat);                        

                        SetAudit(playerStat);
                        playerStat.IsActive = 1;
                        session.SaveOrUpdate(playerStat);
                    }                  

                    transaction.Commit();
                }
            }
        }

        public void SaveFixtureStats(List<PlayerStatViewModel> stats)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    foreach (var playerStatViewModel in stats)
                    {
                        var playerStat = session.CreateCriteria(typeof(PlayerStat))
                       .List<PlayerStat>().FirstOrDefault(x => x.Fixture.Id.Equals(playerStatViewModel.FixtureId)
                           && x.Player.Number.Equals(playerStatViewModel.PlayerNumber)) ??
                                new PlayerStat();

                        if (playerStat.Id == 0)
                        {
                            playerStat.Init();                           
                        }

                        playerStat.Fixture = GetFixture(session, playerStatViewModel.FixtureId);
                        playerStat.Player = GetPlayer(session, playerStatViewModel.PlayerNumber);

                        MapperHelper.Map(playerStat.CricketStat, playerStatViewModel);

                        SetAudit(playerStat.CricketStat);
                        playerStat.CricketStat.IsActive = 1;
                        session.SaveOrUpdate(playerStat.CricketStat);

                        SetAudit(playerStat);
                        playerStat.IsActive = 1;
                        session.SaveOrUpdate(playerStat);
                                                
                       // }
                    }

                    transaction.Commit();
                }
            }
        }    

        public TeamSelectionAdminViewModel GetTeamSelectionAdminViewModel(int id)
        {           
            using (var session = NHibernateHelper.OpenSession())
            {
                var teamSelectionAdminViewModel = new TeamSelectionAdminViewModel();

                var entity = session.CreateCriteria(typeof(TeamSelection))
                        .List<TeamSelection>().FirstOrDefault(x => x.Fixture.Id.Equals(id)) ??
                                 new TeamSelection();

                if (entity.Players==null) entity.Init();

                foreach (var player in entity.Players)
                {                    
                    teamSelectionAdminViewModel.TeamSelection.Add(new PlayerViewModel{Name = player.Name, PlayerNumber = player.Number});
                }

                var selectionCount = entity.Players.Count;

                for (var i = selectionCount; i < 13; i++)
                {
                    teamSelectionAdminViewModel.TeamSelection.Add(new PlayerViewModel());
                }

                return teamSelectionAdminViewModel;
            }            
        }

        public FixtureListViewModel GetFixtureListViewModel()
        {
            return new FixtureListViewModel { Fixtures = GetAll() };
        }

        public AdminFixtureStatViewModel GetAdminFixtureStatViewModel(int fixtureId)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var adminFixtureStatViewModel = new AdminFixtureStatViewModel();

                var playerStatList = session.CreateCriteria(typeof(PlayerStat))
                    .List<PlayerStat>().Where(x=>x.Fixture.Id.Equals(fixtureId)).ToList();

                foreach (var playerStat in playerStatList)
                {
                    adminFixtureStatViewModel.PlayerStats.Add(MapperHelper.Map(new PlayerStatViewModel(), playerStat));
                }

                return adminFixtureStatViewModel;
            }            
        }

        public AdminFixtureReportViewModel GetAdminFixtureReportViewModel(int fixtureId)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var fixture = GetFixture(session, fixtureId);
                var adminFixtureStatViewModel = new AdminFixtureReportViewModel
                {
                    FixtureId = fixtureId,
                    ResultTypes = _configurationServices.GetResultTypes(),
                    ResultType = fixture.ResultType == null ? 0 : fixture.ResultType.Id,
                    FixtureScore = fixture.Score,
                    MatchReport = fixture.MatchReport == null ? string.Empty : fixture.MatchReport.Report,                    
                };

                return adminFixtureStatViewModel;
            }
        }

        public FixtureViewModel GetFixtureViewModel(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var fixture = GetFixture(session, id);
                return MapperHelper.Map(new FixtureViewModel(), fixture);
            }
        }

        public AdminFixtureViewModel GetAdminFixtureViewModel()
        {
            var adminFixtureViewModel = new AdminFixtureViewModel { Fixtures = GetAll() };
            
            return BuildAdminFixtureViewModel(adminFixtureViewModel);  

        }

        public AdminFixtureViewModel GetAdminFixtureViewModel(int fixtureId)
        {
            var adminFixtureViewModel = new AdminFixtureViewModel { Fixtures = new List<FixtureViewModel> { GetFixtureViewModel(fixtureId) } };

            return BuildAdminFixtureViewModel(adminFixtureViewModel);       

        }       

        public AdminFixtureViewModel GetAdminFixtureViewModel(int currentPage, string orderBy, bool orderByReverse, int pageItems,
           string filterBy, string filterByFields)
        {
            int totalCount;
            var adminFixtureViewModel = new AdminFixtureViewModel
            {
                Fixtures = GetPaginated(currentPage, orderBy, orderByReverse, pageItems, filterBy, filterByFields, out totalCount)
            };

            // need to map entity within session becase of lazy-loading
            if (HttpContext.Current.Session["TeamList"] == null)
            {
                var teamList = _configurationServices.GetTeams();                
                adminFixtureViewModel.Teams = (from listItem in teamList
                                               let listItemViewModel = new TeamViewModel()
                                               select MapperHelper.Map(listItemViewModel, listItem)).ToList();
                HttpContext.Current.Session["TeamList"] = adminFixtureViewModel.Teams;

                var opponentList = _configurationServices.GetOpponents();               
                adminFixtureViewModel.Opponents = (from listItem in opponentList
                                                   let listItemViewModel = new OpponentViewModel()
                                                   select MapperHelper.Map(listItemViewModel, listItem)).ToList();
                HttpContext.Current.Session["OpponentList"] = adminFixtureViewModel.Opponents;

                var locationList = _configurationServices.GetLocations();                
                adminFixtureViewModel.Locations = (from listItem in locationList
                                                   let listItemViewModel = new LocationViewModel()
                                                   select MapperHelper.Map(listItemViewModel, listItem)).ToList();
                HttpContext.Current.Session["LocationList"] = adminFixtureViewModel.Locations;

                var fixtureTypeList = _configurationServices.GetFixtureTypes();                
                adminFixtureViewModel.FixtureTypes = (from listItem in fixtureTypeList
                                                      let listItemViewModel = new FixtureTypeViewModel()
                                                      select MapperHelper.Map(listItemViewModel, listItem)).ToList();
                HttpContext.Current.Session["FixtureTypeList"] = adminFixtureViewModel.FixtureTypes;

            }
            else
            {
                adminFixtureViewModel.Teams = (List<TeamViewModel>)HttpContext.Current.Session["TeamList"];
                adminFixtureViewModel.Opponents = (List<OpponentViewModel>)HttpContext.Current.Session["OpponentList"];
                adminFixtureViewModel.Locations = (List<LocationViewModel>)HttpContext.Current.Session["LocationList"];
                adminFixtureViewModel.FixtureTypes = (List<FixtureTypeViewModel>)HttpContext.Current.Session["FixtureTypeList"];
            }

            //adminFixtureViewModel.HowOut = (from listItem in howOutList
            //                                      let listItemViewModel = new HowOutViewModel()
            //                                      select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            adminFixtureViewModel.HomeOrAway.Add(HomeAway.Home.ToString());
            adminFixtureViewModel.HomeOrAway.Add(HomeAway.Away.ToString());

            return adminFixtureViewModel;
        }

        private Player GetPlayer(ISession session, string playerNumber)
        {
            return session.CreateCriteria(typeof (Player))
                .List<Player>().FirstOrDefault(x => x.Number.Equals(playerNumber)) ??
                   new Player();
        }

        private Fixture GetFixture(ISession session, int fixtureId)
        {
            return session.CreateCriteria(typeof(Fixture))
                .List<Fixture>().FirstOrDefault(x => x.Id.Equals(fixtureId)) ??
                   new Fixture();
        }       

        private PlayerStat GetPlayerStat(ISession session, int fixtureId, string playerNumber)
        {
            var playerStat = session.CreateCriteria(typeof(PlayerStat))
                    .List<PlayerStat>().FirstOrDefault(x => x.Fixture.Id.Equals(fixtureId) && x.Player.Number.Equals(playerNumber)) ??
                             new PlayerStat();

            if (playerStat.CricketStat == null) playerStat.CricketStat = new CricketStat();
            if (playerStat.Player == null) playerStat.Player = GetPlayer(session, playerNumber);
            if (playerStat.Fixture == null) playerStat.Fixture = GetFixture(session, fixtureId);            

            return playerStat;
        }

        private AdminFixtureViewModel BuildAdminFixtureViewModel(AdminFixtureViewModel adminFixtureViewModel)
        {
            // need to map entity within session becase of lazy-loading
            var teamList = _configurationServices.GetTeams();
            var opponentList = _configurationServices.GetOpponents();
            var locationList = _configurationServices.GetLocations();
            var fixtureTypeList = _configurationServices.GetFixtureTypes();
            var resultTypeList = _configurationServices.GetResultTypes();
            //var howOutList = _configurationServices.GetHowOut();            

            adminFixtureViewModel.Teams = (from listItem in teamList
                                           let listItemViewModel = new TeamViewModel()
                                           select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            adminFixtureViewModel.Opponents = (from listItem in opponentList
                                               let listItemViewModel = new OpponentViewModel()
                                               select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            adminFixtureViewModel.Locations = (from listItem in locationList
                                               let listItemViewModel = new LocationViewModel()
                                               select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            adminFixtureViewModel.FixtureTypes = (from listItem in fixtureTypeList
                                                  let listItemViewModel = new FixtureTypeViewModel()
                                                  select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            adminFixtureViewModel.ResultTypes = (from listItem in resultTypeList
                                                  let listItemViewModel = new ResultTypeViewModel()
                                                  select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            //adminFixtureViewModel.HowOut = (from listItem in howOutList
            //                                      let listItemViewModel = new HowOutViewModel()
            //                                      select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            adminFixtureViewModel.HomeOrAway.Add(HomeAway.Home.ToString());
            adminFixtureViewModel.HomeOrAway.Add(HomeAway.Away.ToString());

            return adminFixtureViewModel;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ivNet.Club.Entities;
using ivNet.Club.Enums;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
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
        AdminFixtureViewModel GetAdminFixtureViewModel();
        void SaveFixture(FixtureViewModel item);
        void SaveTeamSelection(int fixtureId, AdminTeamSelectionViewModel teamSelectionViewModel);

        TeamSelectionAdminViewModel GetTeamSelectionAdminViewModel(int id);
        FixtureListViewModel GetFixtureViewModel();
        AdminFixtureStatViewModel GetAdminFixtureStatViewModel(int fixtureId);
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

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entity = session.CreateCriteria(typeof (Fixture))
                        .List<Fixture>().FirstOrDefault(x => x.Id.Equals(item.Id)) ?? new Fixture();

                    var team = session.CreateCriteria(typeof(Team))
                       .List<Team>().FirstOrDefault(x => x.Name.Equals(item.Team));

                    var opponent = session.CreateCriteria(typeof (Opponent))
                        .List<Opponent>().FirstOrDefault(x => x.Name.Equals(item.Opponent));

                    var location = session.CreateCriteria(typeof (Location))
                        .List<Location>().FirstOrDefault(x => x.Postcode.Equals(item.Location));

                    var fixtureType = session.CreateCriteria(typeof (FixtureType))
                        .List<FixtureType>().FirstOrDefault(x => x.Name.Equals(item.FixtureType));

                    entity.Date = item.Date.GetValueOrDefault();
                    entity.FixtureKey = CustomStringHelper.BuildKey(new[] {team.Name, entity.Date.ToShortDateString()});
                    entity.FixtureType = fixtureType;
                    entity.HomeAway = item.HomeAway;
                    entity.Location = location;
                    entity.Opponent = opponent;
                    entity.Team = team;

                    entity.IsActive = 1;

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);

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

        public FixtureListViewModel GetFixtureViewModel()
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

        public AdminFixtureViewModel GetAdminFixtureViewModel()
        {
            var adminFixtureViewModel = new AdminFixtureViewModel { Fixtures = GetAll() };
            // need to map entity within session becase of lazy-loading
            var teamList = _configurationServices.GetTeams();
            var opponentList = _configurationServices.GetOpponents();
            var locationList = _configurationServices.GetLocations();
            var fixtureTypeList = _configurationServices.GetFixtureTypes();
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

    }
}
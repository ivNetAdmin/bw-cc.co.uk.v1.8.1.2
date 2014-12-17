using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ivNet.Club.Entities;
using ivNet.Club.Enums;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
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
                    var entity = session.CreateCriteria(typeof (TeamSelection))
                        .List<TeamSelection>().FirstOrDefault(x => x.Id.Equals(teamSelectionViewModel.TeamSelectionId)) ??
                                 new TeamSelection();
                                      
                    if (entity.Players == null)
                    {
                        entity.Init();
                    }
                    else
                    {
                        foreach (var player in entity.Players)
                        {
                            entity.RemovePlayer(player);
                        }
                    }

                    entity.Fixture = session.CreateCriteria(typeof(Fixture))
                       .List<Fixture>().FirstOrDefault(x => x.Id.Equals(fixtureId));

                    foreach (var playerViewModel 
                        in teamSelectionViewModel.TeamSelection.Where(
                            playerViewModel => !string.IsNullOrEmpty(playerViewModel.PlayerNumber)))
                    {
                        entity.AddPlayer(
                            session.CreateCriteria(typeof (Player))
                                .List<Player>().FirstOrDefault(x => x.Number.Equals(playerViewModel.PlayerNumber))
                            );
                    }

                    entity.IsActive = 1;

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);

                    transaction.Commit();
                }
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

            adminFixtureViewModel.HomeOrAway.Add(HomeAway.Home.ToString());
            adminFixtureViewModel.HomeOrAway.Add(HomeAway.Away.ToString());

            return adminFixtureViewModel;

        }
    }
}
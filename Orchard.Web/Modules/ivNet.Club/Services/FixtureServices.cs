using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ivNet.Club.Entities;
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
        void SaveFixture(FixtureViewModel item);
    }

    public class FixtureServices : BaseService, IFixtureServices
    {
        public FixtureServices(IMembershipService membershipService, 
            IAuthenticationService authenticationService, 
            IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository) 
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
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
    }
}
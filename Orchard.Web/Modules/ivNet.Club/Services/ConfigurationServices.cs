
using System.Collections.Generic;
using System.Linq;
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
    public interface IConfigurationServices : IDependency
    {
        List<ConfigurationItem> Get();
        void Save(int id, ConfigurationItemViewModel item);
    }

    public class ConfigurationServices : BaseService, IConfigurationServices
    {
        public ConfigurationServices(IMembershipService membershipService, 
            IAuthenticationService authenticationService, 
            IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository) 
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
        }

        public List<ConfigurationItem> Get()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria(typeof (ConfigurationItem))
                    .List<ConfigurationItem>().ToList();
            }
        }

        public void Save(int id, ConfigurationItemViewModel viewModel)
        {

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entity = session.CreateCriteria(typeof (ConfigurationItem))
                        .List<ConfigurationItem>().FirstOrDefault(x => x.Id.Equals(id)) ?? new ConfigurationItem();

                    MapperHelper.Map(entity, viewModel);

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);
                       

                    transaction.Commit();
                }
            }            
        }
    }
}
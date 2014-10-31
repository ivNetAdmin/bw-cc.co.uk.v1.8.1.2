
using System.Collections.Generic;
using System.Linq;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using Orchard;

namespace ivNet.Club.Services
{
    public interface IConfigurationServices : IDependency
    {
        List<ConfigurationItem> Get();
    }

    public class ConfigurationServices : IConfigurationServices
    {
        public List<ConfigurationItem> Get()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria(typeof (ConfigurationItem))
                    .List<ConfigurationItem>().ToList();
            }
        }
    }
}
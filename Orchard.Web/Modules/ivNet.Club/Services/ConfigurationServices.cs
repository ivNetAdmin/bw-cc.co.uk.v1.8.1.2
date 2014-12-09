
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
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
        string GetCurrentSeason();
        List<decimal> GetFeeData();
        int GetJuniorYear(DateTime dob);
        IEnumerable<ConfigurationItem> GetExtraFeeData();
        IEnumerable<ListItemViewModel> GetRegistrationSeasonList();
        IEnumerable<Team> GetTeams();
        IEnumerable<Opponent> GetOpponents();
        IEnumerable<FixtureType> GetFixtureTypes();
        IEnumerable<Location> GetLocations();
        void SaveTeam(int id, string name, byte isActive);
        void SaveOpponent(int id, string name, byte isActive);
        void SaveFixtureType(int id, string name, byte isActive);
        void SaveLocation(int id, string name, string postcode,  decimal latitude, decimal longitude, byte isActive);
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

        public void SaveTeam(int id, string name, byte isActive)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entity = session.CreateCriteria(typeof(Team))
                        .List<Team>().FirstOrDefault(x => x.Id.Equals(id)) ?? new Team();

                    entity.Name = name;
                    entity.IsActive = isActive;

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);

                    transaction.Commit();
                }
            }          
        }

        public void SaveOpponent(int id, string name, byte isActive)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entity = session.CreateCriteria(typeof(Opponent))
                        .List<Opponent>().FirstOrDefault(x => x.Id.Equals(id)) ?? new Opponent();

                    entity.Name = name;
                    entity.IsActive = isActive;

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);

                    transaction.Commit();
                }
            }        
        }

        public void SaveFixtureType(int id, string name, byte isActive)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entity = session.CreateCriteria(typeof(FixtureType))
                        .List<FixtureType>().FirstOrDefault(x => x.Id.Equals(id)) ?? new FixtureType();

                    entity.Name = name;
                    entity.IsActive = isActive;

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);

                    transaction.Commit();
                }
            }        
        }

        public void SaveLocation(int id, string name, string postcode, decimal latitude, decimal longitude, byte isActive)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entity = session.CreateCriteria(typeof(Location))
                        .List<Location>().FirstOrDefault(x => x.Id.Equals(id)) ?? new Location();

                    entity.Name = name;
                    entity.IsActive = isActive;
                    entity.Postcode = postcode;
                    entity.Latitude = latitude;
                    entity.Longitude = longitude;

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);

                    transaction.Commit();
                }
            }        
        }

        public string GetCurrentSeason()
        {
            if (HttpContext.Current.Cache["currentSeason"] != null) return HttpContext.Current.Cache["currentSeason"].ToString();

            using (var session = NHibernateHelper.OpenSession())
            {
                
                DateTime? seasonStartDate = null;
                DateTime? seasonEndDate = null;
                string seasonFormat = string.Empty;
                int currentseason = DateTime.Now.Year;

                var seasonItems = session.CreateCriteria(typeof (ConfigurationItem))
                    .List<ConfigurationItem>().Where(x => x.ItemGroup.Equals("Season"));

                foreach (var configurationItem in seasonItems)
                {
                    switch (configurationItem.Name.Replace(" ",string.Empty).ToLowerInvariant())
                    {
                        case "displayformat":
                            seasonFormat = configurationItem.Text;
                            break;
                        case "startdate":
                            seasonStartDate = configurationItem.Date;
                            break;
                        case "enddate":
                            seasonEndDate = configurationItem.Date;
                            break;
                    }
                }

                if (seasonStartDate != null && seasonEndDate != null)               
                {                   
                    if (DateTime.Now.Month > seasonEndDate.GetValueOrDefault().Month && DateTime.Now.Month < seasonStartDate.GetValueOrDefault().Month)
                    {
                        currentseason++;
                    }
                }

                HttpContext.Current.Cache["currentSeason"] = seasonFormat == "####/##" ? string.Format("{0}/{1}", 
                    currentseason, (currentseason + 1).ToString(CultureInfo.InvariantCulture).Substring(2)) : 
                    currentseason.ToString(CultureInfo.InvariantCulture);
                return HttpContext.Current.Cache["currentSeason"].ToString();
            }            
        }

        public List<decimal> GetFeeData()
        {           
            using (var session = NHibernateHelper.OpenSession())
            {
                var returnList = new List<decimal>();
                decimal fee1 = 0;
                decimal fee2 = 0;
                decimal lowFeeAge = 0;

                var feeItems = session.CreateCriteria(typeof (ConfigurationItem))
                    .List<ConfigurationItem>().Where(x => x.ItemGroup.Equals("Registration")).OrderBy(x=>x.Number);

                foreach (var configurationItem in feeItems)
                {
                    switch (configurationItem.Name.Replace(" ", string.Empty).ToLowerInvariant())
                    {
                        case "lowfeeage":
                            lowFeeAge = configurationItem.Number;
                            break;
                        case "fee1":
                            fee1 = configurationItem.Number;
                            break;
                        case "fee2":
                            fee2 = configurationItem.Number;
                            break;
                    }
                }

                returnList.Add(lowFeeAge);
                returnList.Add(fee1);
                returnList.Add(fee2);

                return returnList;
            }
        }

        public IEnumerable<ConfigurationItem> GetExtraFeeData()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var returnList = new List<ConfigurationItem>();

                var feeItems = session.CreateCriteria(typeof (ConfigurationItem))
                    .List<ConfigurationItem>().Where(x => x.ItemGroup.Equals("Registration")).OrderBy(x => x.Number);

                foreach (var configurationItem in feeItems)
                {
                    switch (configurationItem.Name.Replace(" ", string.Empty).ToLowerInvariant())
                    {
                        case "kit":
                        case "lottery":
                            returnList.Add(configurationItem);
                            break;
                    }
                }

                return returnList;
            }
        }

        public IEnumerable<ListItemViewModel> GetRegistrationSeasonList()
        {            
            using (var session = NHibernateHelper.OpenSession())
            {

                DateTime? seasonStartDate = null;
                DateTime? seasonEndDate = null;
                var seasonFormat = string.Empty;
                var currentseason = DateTime.Now.Year;

                var seasonItems = session.CreateCriteria(typeof(ConfigurationItem))
                    .List<ConfigurationItem>().Where(x => x.ItemGroup.Equals("Season"));

                foreach (var configurationItem in seasonItems)
                {
                    switch (configurationItem.Name.Replace(" ", string.Empty).ToLowerInvariant())
                    {
                        case "displayformat":
                            seasonFormat = configurationItem.Text;
                            break;
                        case "startdate":
                            seasonStartDate = configurationItem.Date;
                            break;
                        case "enddate":
                            seasonEndDate = configurationItem.Date;
                            break;
                    }
                }

                if (seasonStartDate != null && seasonEndDate != null)
                {
                    if (DateTime.Now.Month > seasonEndDate.GetValueOrDefault().Month && DateTime.Now.Month < seasonStartDate.GetValueOrDefault().Month)
                    {
                        currentseason++;
                    }
                }

                var rtnList = new List<ListItemViewModel>();

                for (var i = 0; i < 3; i++)
                {
                    var selected = i == 0;

                    var season = seasonFormat == "####/##"
                        ? string.Format("{0}/{1}",
                            currentseason+i, (currentseason + i+1).ToString(CultureInfo.InvariantCulture).Substring(2))
                        : (currentseason+i).ToString(CultureInfo.InvariantCulture);

                    rtnList.Add(new ListItemViewModel { Value = season, Text = season, Selected = selected });
     
                }            

                return rtnList;
            }            

        }

        public IEnumerable<Team> GetTeams()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria(typeof (Team))
                    .List<Team>().OrderBy(x => x.Name);
            }
        }

        public IEnumerable<Opponent> GetOpponents()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria(typeof(Opponent))
                    .List<Opponent>().OrderBy(x => x.Name);
            }
        }

        public IEnumerable<FixtureType> GetFixtureTypes()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria(typeof(FixtureType))
                    .List<FixtureType>().OrderBy(x => x.Name);
            }

        }

        public IEnumerable<Location> GetLocations()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria(typeof(Location))
                    .List<Location>().OrderBy(x => x.Name);
            }
        }

        public int GetJuniorYear(DateTime dob)
        {
            if (HttpContext.Current.Cache["juniourTeamDate"] == null)
            {

                using (var session = NHibernateHelper.OpenSession())
                {
                    var entity = session.CreateCriteria(typeof (ConfigurationItem))
                        .List<ConfigurationItem>()
                        .FirstOrDefault(
                            x => x.Name.Replace(" ", string.Empty).ToLowerInvariant().Equals("juniorteamdate"));

                    if (entity == null) return 0;

                    HttpContext.Current.Cache["juniourTeamDate"] = entity.Date;

                }
            }

            var juniourTeamDate = (DateTime?) HttpContext.Current.Cache["juniourTeamDate"];
            // at the 01/01 this year junior would be
            var age = DateTime.Now.Year - dob.Year;

            // if junior's birthday is after juniourTeamDate then junior will play a year older
            if (dob.Month > juniourTeamDate.GetValueOrDefault().Month)
            {
                age++;
            }
            else if (dob.Month == juniourTeamDate.GetValueOrDefault().Month &&
                     dob.Day >= juniourTeamDate.GetValueOrDefault().Day)
            {
                age++;
            }

            return age;           
        }
    }
}
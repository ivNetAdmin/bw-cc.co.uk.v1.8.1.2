﻿
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentNHibernate.Data;
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

        public string GetCurrentSeason()
        {
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

                return seasonFormat == "####/##" ? string.Format("{0}/{1}", 
                    currentseason, (currentseason + 1).ToString(CultureInfo.InvariantCulture).Substring(2)) : 
                    currentseason.ToString(CultureInfo.InvariantCulture);
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

        public int GetJuniorYear(DateTime dob)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var entity = session.CreateCriteria(typeof (ConfigurationItem))
                    .List<ConfigurationItem>().FirstOrDefault(x => x.Name.Replace(" ",string.Empty).ToLowerInvariant().Equals("juniorteamdate"));

                if (entity == null) return 0;

                var juniourTeamDate = entity.Date;
                
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
}
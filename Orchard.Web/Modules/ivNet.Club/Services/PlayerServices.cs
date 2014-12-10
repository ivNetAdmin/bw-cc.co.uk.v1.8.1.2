

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using NHibernate.Criterion;
using Orchard;
using Orchard.Data;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;

namespace ivNet.Club.Services
{

    public interface IPlayerServices : IDependency
    {
        List<JuniorNewRegistrationFeeViewModel> GetCachedJuniorRegistrations();
    }

    public class PlayerServices : BaseService, IPlayerServices
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IConfigurationServices _configurationServices;

        public PlayerServices(
            IConfigurationServices configurationServices,
            IWorkContextAccessor workContextAccessor,
            IMembershipService membershipService,
            IAuthenticationService authenticationService,
            IRoleService roleService,
            IRepository<UserRolesPartRecord> userRolesRepository)
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
            _configurationServices = configurationServices;
            _workContextAccessor = workContextAccessor;
        }

        public List<JuniorNewRegistrationFeeViewModel> GetCachedJuniorRegistrations()
        {
            var currentSeason = _configurationServices.GetCurrentSeason();

            using (var session = NHibernateHelper.OpenSession())
            {
                var memberIdList = ItemsInternal;

                var juniorList = session
                   .CreateCriteria(typeof(Junior))
                   .Add(Restrictions.In("Member.Id", memberIdList))
                   .List<Junior>();

                var juniorNewRegistrationFeeViewModelList = (
                        from junior in juniorList
                        let juniorNewRegistrationFeeViewModel = new JuniorNewRegistrationFeeViewModel()
                        select MapperHelper.Map(juniorNewRegistrationFeeViewModel, junior, currentSeason)).OrderBy(vm => vm.Dob).ToList();

                AddFees(juniorNewRegistrationFeeViewModelList);

                return juniorNewRegistrationFeeViewModelList;        
            }           
        }

        public void Clear()
        {
            ItemsInternal.Clear();
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        private List<int> ItemsInternal
        {
            get
            {
                var items = (List<int>)HttpContext.Session["NewRegistrations"];

                if (items != null) return items;
                items = new List<int>();
                HttpContext.Session["NewRegistrations"] = items;

                return items;
            }
        }


        private void AddFees(List<JuniorNewRegistrationFeeViewModel> juniorNewRegistrationFeeViewModelList)
        {
            var fees = _configurationServices.GetFeeData();

            if (fees.Count == 0) return;

            for (var i = 0; i < juniorNewRegistrationFeeViewModelList.Count(); i++)
            {
                var juniorYear = GetJuniorYear(juniorNewRegistrationFeeViewModelList[i].Dob);
                juniorNewRegistrationFeeViewModelList[i].Team = string.Format("U{0}", juniorYear);
                if (i == 0)
                {
                    juniorNewRegistrationFeeViewModelList[i].Fee = fees[1];
                    if (juniorYear <= fees[0])
                    {
                        juniorNewRegistrationFeeViewModelList[i].Fee = fees[2];
                    }
                }
                else if (i == 1)
                {
                    juniorNewRegistrationFeeViewModelList[i].Fee = fees[2];
                }
            }
        }

        private int GetJuniorYear(DateTime dob)
        {
            return _configurationServices.GetJuniorYear(dob);
        }

        //      public List<JuniorRegistrationViewModel> Get()
        //{
        //    var currentSeason = _configurationServices.GetCurrentSeason();

        //    using (var session = NHibernateHelper.OpenSession())
        //    {
        //        var memberIdList = ItemsInternal;

        //        var juniorList = session
        //            .CreateCriteria(typeof (Junior))
        //            .Add(Restrictions.In("Member.Id", memberIdList))
        //            .List<Junior>();

        //        var juniorRegistrationViewModelList =  (
        //            from junior in juniorList
        //            let juniorRegistrationViewModel = new JuniorRegistrationViewModel()
        //            select MapperHelper.Map(juniorRegistrationViewModel, junior, currentSeason)).OrderBy(vm => vm.Dob).ToList();

        //        AddFees(juniorRegistrationViewModelList);

        //        return juniorRegistrationViewModelList;
        //    }          
        //}
        
    }
}
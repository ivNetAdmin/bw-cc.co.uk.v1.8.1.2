
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
    public interface IRegistrationServices : IDependency
    {
        void Add(int memberId);
        List<JuniorRegistrationViewModel> Get();
        void Clear();
        List<JuniorVettingViewModel> GetNonVetted();
        void Activate(int id, JuniorVettingViewModel item);
    }

    public class RegistrationServices : BaseService, IRegistrationServices
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IConfigurationServices _configurationServices;

        public RegistrationServices(
            IMembershipService membershipService, 
            IAuthenticationService authenticationService, 
            IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository,
            IWorkContextAccessor workContextAccessor, 
            IConfigurationServices configurationServices)
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {
            _workContextAccessor = workContextAccessor;
            _configurationServices = configurationServices;
        }

        public void Add(int memberId)
        {
            ItemsInternal.Add(memberId);
        }

        public List<JuniorRegistrationViewModel> Get()
        {
            var currentSeason = _configurationServices.GetCurrentSeason();

            using (var session = NHibernateHelper.OpenSession())
            {
                var memberIdList = ItemsInternal;

                var juniorList = session
                    .CreateCriteria(typeof (Junior))
                    .Add(Restrictions.In("Member.Id", memberIdList))
                    .List<Junior>();

                var juniorRegistrationViewModelList =  (
                    from junior in juniorList
                    let juniorRegistrationViewModel = new JuniorRegistrationViewModel()
                    select MapperHelper.Map(juniorRegistrationViewModel, junior, currentSeason)).OrderBy(vm => vm.Dob).ToList();

                AddFees(juniorRegistrationViewModelList);

                return juniorRegistrationViewModelList;
            }          
        }

        public List<JuniorVettingViewModel> GetNonVetted()
        {

            using (var session = NHibernateHelper.OpenSession())
            {
                var juniorList = session.CreateCriteria(typeof (Junior))
                    .List<Junior>().Where(x => x.IsVetted.Equals(0)).ToList();

                return (from junior in juniorList
                    let juniorVettingViewModel = new JuniorVettingViewModel()
                        select MapperHelper.Map(_configurationServices, juniorVettingViewModel, junior)).ToList();

            }            
        }

        public void Activate(int id, JuniorVettingViewModel item)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entity = session.CreateCriteria(typeof(Junior))
                        .List<Junior>().FirstOrDefault(x => x.Id.Equals(id));

                    entity.IsVetted = item.IsVetted;

                    SetAudit(entity);
                    session.SaveOrUpdate(entity);

                    transaction.Commit();
                }
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

        private void AddFees (List<JuniorRegistrationViewModel> juniorRegistrationViewModelList)
        {
            var fees = _configurationServices.GetFeeData();

            if (fees.Count == 0) return;

            for(var i=0;i<juniorRegistrationViewModelList.Count();i++)
            {
                var juniorYear = GetJuniorYear(juniorRegistrationViewModelList[i].Dob);
                juniorRegistrationViewModelList[i].Team = string.Format("U{0}", juniorYear);
                if (i == 0)
                {
                    juniorRegistrationViewModelList[i].Fee = fees[1];
                    if (juniorYear <= fees[0])
                    {
                        juniorRegistrationViewModelList[i].Fee = fees[2];
                    }
                }else if (i == 1)
                {
                    juniorRegistrationViewModelList[i].Fee = fees[2];
                }
            }
        }

        private int GetJuniorYear(DateTime dob)
        {
           return _configurationServices.GetJuniorYear(dob);
        }
    }
}
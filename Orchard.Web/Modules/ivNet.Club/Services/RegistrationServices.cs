
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using NHibernate.Criterion;
using Orchard;

namespace ivNet.Club.Services
{
    public interface IRegistrationServices : IDependency
    {
        void Add(int memberId);
        List<JuniorRegistrationViewModel> Get();
        void Clear();
    }

    public class RegistrationServices : IRegistrationServices
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IConfigurationServices _configurationServices;

        public RegistrationServices(IWorkContextAccessor workContextAccessor, IConfigurationServices configurationServices)           
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
                    .Add(Restrictions.In("ClubMember.Id", memberIdList))
                    .List<Junior>();

                var juniorRegistrationViewModelList =  (
                    from junior in juniorList
                    let juniorRegistrationViewModel = new JuniorRegistrationViewModel()
                    select MapperHelper.Map(juniorRegistrationViewModel, junior, currentSeason)).OrderBy(vm => vm.Dob).ToList();

                AddFees(juniorRegistrationViewModelList);

                return juniorRegistrationViewModelList;
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
                    juniorRegistrationViewModelList[i].Fee = fees[2];
                    if (juniorYear <= fees[0])
                    {
                        juniorRegistrationViewModelList[i].Fee = fees[1];
                    }
                }else if (i == 1)
                {
                    juniorRegistrationViewModelList[i].Fee = fees[1];
                }
            }
        }

        private int GetJuniorYear(DateTime dob)
        {
           return _configurationServices.GetJuniorYear(dob);
        }
    }
}
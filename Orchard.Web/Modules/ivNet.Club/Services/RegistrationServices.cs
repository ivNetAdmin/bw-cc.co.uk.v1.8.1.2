
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

        public RegistrationServices(IWorkContextAccessor workContextAccessor)           
        {
            _workContextAccessor = workContextAccessor;
        }

        public void Add(int memberId)
        {
            ItemsInternal.Add(memberId);
        }

        public List<JuniorRegistrationViewModel> Get()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var memberIdList = ItemsInternal;

                var juniorList = session
                    .CreateCriteria(typeof (Junior))
                    .Add(Restrictions.In("ClubMember.Id", memberIdList))
                    .List<Junior>();
             
                return (
                    from junior in juniorList 
                    let juniorRegistrationViewModel = new JuniorRegistrationViewModel() 
                    select MapperHelper.Map(juniorRegistrationViewModel, junior)).ToList();
            }
            
            // return list of members who have just been registered and their fee details

 
            //return ItemsInternal;
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
    }
}
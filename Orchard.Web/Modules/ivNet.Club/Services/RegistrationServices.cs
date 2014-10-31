
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
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
                var juniorRegistrationList = new List<JuniorRegistrationViewModel>();

                var juniorList = session.CreateCriteria(typeof(Junior)).List<Junior>();

                foreach (var junior in juniorList)
                {
                    foreach (var memberId in memberIdList)
                    {
                        if (junior.ClubMember.Id == memberId)
                        {
                            var juniorRegistrationViewModel = new JuniorRegistrationViewModel();
                            juniorRegistrationList.Add(MapperHelper.Map(juniorRegistrationViewModel, junior));       
                        }
                    }

                    
                }
                return juniorRegistrationList;
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
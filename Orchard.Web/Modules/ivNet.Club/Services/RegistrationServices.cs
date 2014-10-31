
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using Orchard;

namespace ivNet.Club.Services
{
    public interface IRegistrationServices : IDependency
    {
        void Add(int memberId);
        List<Junior> Get();
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

        public List<Junior> Get()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var memberList = session.CreateCriteria(typeof(Junior)).List<Junior>();
                return memberList.ToList();
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
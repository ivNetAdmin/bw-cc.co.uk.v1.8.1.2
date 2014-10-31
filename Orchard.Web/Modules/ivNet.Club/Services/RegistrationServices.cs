
using System.Collections.Generic;
using System.Web;
using ivNet.Club.Entities;
using Orchard;

namespace ivNet.Club.Services
{
    public interface IRegistrationServices : IDependency
    {
        void Add(ClubMember memberId);
        List<ClubMember> Get();
        void Clear();
    }

    public class RegistrationServices : IRegistrationServices
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public RegistrationServices(IWorkContextAccessor workContextAccessor)           
        {
            _workContextAccessor = workContextAccessor;
        }

        public void Add(ClubMember member)
        {
            ItemsInternal.Add(member);
        }

        public List<ClubMember> Get()
        {
            return ItemsInternal;
        }

        public void Clear()
        {
            ItemsInternal.Clear();
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        private List<ClubMember> ItemsInternal
        {
            get
            {
                var items = (List<ClubMember>)HttpContext.Session["NewRegistrations"];

                if (items != null) return items;
                items = new List<ClubMember>();
                HttpContext.Session["NewRegistrations"] = items;

                return items;
            }
        }
    }
}

using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class AdminContactController : ApiController
    {

        private readonly IMemberServices _memberServices;
        private readonly IOrchardServices _orchardServices;

        public AdminContactController(IMemberServices memberServices, IOrchardServices orchardServices)
        {
            _memberServices = memberServices;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            return Request.CreateResponse(HttpStatusCode.OK,
                GetContactAdminViewModel());
        }

        public HttpResponseMessage Get(int currentPage, string orderBy, bool orderByReverse, int pageItems, string filterBy, string filterByFields, string ageGroup)
        {
            int totalItems;
            var contacts = _memberServices.GetPaginatedJuniorContacts(currentPage, orderBy, orderByReverse, pageItems, filterBy, filterByFields, ageGroup, out totalItems );
            var json = new {contacts = contacts, totalItems = totalItems};        
            return Request.CreateResponse(HttpStatusCode.OK, json);
        }

        private ContactAdminViewModel GetContactAdminViewModel()
        {
            return _memberServices.GetContactAdminViewModel();
        }
    }
}
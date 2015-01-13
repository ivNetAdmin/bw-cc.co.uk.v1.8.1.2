
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;
using Orchard;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class BwccDataLoadController : ApiController
    {
        private readonly IBwccDataServices _bwccDataServices;
        private readonly IFixtureServices _fixtureServices;
        private readonly IOrchardServices _orchardServices;        
        private readonly IPlayerServices _playerServices;

        public BwccDataLoadController(IBwccDataServices bwccDataServices, IFixtureServices fixtureServices, IOrchardServices orchardServices,
            IPlayerServices playerServices)
        {
            _bwccDataServices = bwccDataServices;
            _fixtureServices = fixtureServices;
            _orchardServices = orchardServices;
            _playerServices = playerServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            return Request.CreateResponse(HttpStatusCode.OK,
                LoadData());
        }

        private string LoadData()
        {
            // get members
            var members = _bwccDataServices.GetMembers();
            return "Data loaded";
        }
    }
}
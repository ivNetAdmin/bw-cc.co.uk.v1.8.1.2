
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class AdminFixtureReportController  : ApiController
    {
        private readonly IFixtureServices _fixtureServices;
        private readonly IOrchardServices _orchardServices;
        private readonly IConfigurationServices _configurationServices;

        public AdminFixtureReportController(IFixtureServices fixtureServices, IOrchardServices orchardServices,
            IConfigurationServices configurationServices)
        {
            _fixtureServices = fixtureServices;
            _orchardServices = orchardServices;
            _configurationServices = configurationServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            var fixtureStat = GetFixtureReport(id);
          

            return Request.CreateResponse(HttpStatusCode.OK,
                fixtureStat);
        }

        private AdminFixtureReportViewModel GetFixtureReport(int fixtureId)
        {
            return _fixtureServices.GetAdminFixtureReportViewModel(fixtureId);
        }
    }
}
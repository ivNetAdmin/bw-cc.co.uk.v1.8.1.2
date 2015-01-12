
using System;
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

            var fixtureReport = GetFixtureReport(id);          

            return Request.CreateResponse(HttpStatusCode.OK,
                fixtureReport);
        }

        private AdminFixtureReportViewModel GetFixtureReport(int fixtureId)
        {
            return _fixtureServices.GetAdminFixtureReportViewModel(fixtureId);
        }


        [HttpPut]
        public HttpResponseMessage Put(int id, AdminFixtureReportViewModel matchReport)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            try
            {
                _fixtureServices.SaveMatchReport(id, matchReport);

                return Request.CreateResponse(HttpStatusCode.OK,
                  "Success");
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                Logger.Error(string.Format("{0}: {1}{2} [{3}]", Request.RequestUri, ex.Message,
                    ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "An Error has occurred. Report to bp@ivnet.co.uk quoting: " + errorId);

            }
        }

      
    }
}

using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ivNet.Club.Controllers.Api
{
    public class AdminFixtureController : ApiController
    {
         private readonly IFixtureServices _fixtureServices;
         private readonly IOrchardServices _orchardServices;
         private readonly IConfigurationServices _configurationServices;

        public AdminFixtureController(IFixtureServices fixtureServices, IOrchardServices orchardServices,
            IConfigurationServices configurationServices)
        {
            _fixtureServices = fixtureServices;
            _orchardServices = orchardServices;
            _configurationServices = configurationServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            return Request.CreateResponse(HttpStatusCode.OK,
                GetFixtures());
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, FixtureViewModel item)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);           

            try
            {
                item.Id = id;

                _fixtureServices.SaveFixture(item);

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

        private AdminFixtureViewModel GetFixtures()
        {
            var adminFixtureViewModel = _fixtureServices.GetAdminFixtureViewModel();
                adminFixtureViewModel.Fixtures.Insert(0, new FixtureViewModel());
            return adminFixtureViewModel;
        }
    }
}
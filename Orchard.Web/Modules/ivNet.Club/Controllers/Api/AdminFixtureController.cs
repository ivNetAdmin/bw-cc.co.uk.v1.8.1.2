
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Helpers;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class AdminFixtureController : ApiController
    {
         private readonly IFixtureServices _fixtureServices;
         private readonly IOrchardServices _orchardServices;

         public AdminFixtureController(IFixtureServices fixtureServices, IOrchardServices orchardServices)
        {
            _fixtureServices = fixtureServices;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            var fixtureList = _fixtureServices.GetAll();

            var rtnList = fixtureList.Select(fixture => MapperHelper.Map(new FixtureViewModel(), fixture)).ToList();

            return Request.CreateResponse(HttpStatusCode.OK,
                rtnList);
        }
    }
}
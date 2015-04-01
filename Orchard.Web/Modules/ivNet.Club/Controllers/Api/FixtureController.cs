
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class FixtureController : ApiController
    {
         private readonly IFixtureServices _fixtureServices;
   
         public FixtureController(IFixtureServices fixtureServices)
        {
            _fixtureServices = fixtureServices;
          
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {           
            return Request.CreateResponse(HttpStatusCode.OK,
                GetFixtures());
        }

        private FixtureListViewModel GetFixtures()
        {
            return _fixtureServices.GetFixtureListViewModel();          
        }
    }
}
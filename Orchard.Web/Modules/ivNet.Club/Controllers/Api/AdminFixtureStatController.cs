
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Entities;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class AdminFixtureStatController : ApiController
    {
        private readonly IFixtureServices _fixtureServices;
        private readonly IOrchardServices _orchardServices;
        private readonly IConfigurationServices _configurationServices;

        public AdminFixtureStatController(IFixtureServices fixtureServices, IOrchardServices orchardServices,
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

            var fixtureStat= GetFixtureStat(id);

            for (var i = fixtureStat.PlayerStats.Count; i < 13; i++)
            {
                var playerStat = new PlayerStatViewModel();        
                fixtureStat.PlayerStats.Add(playerStat);
            }

            fixtureStat.HowOut = _configurationServices.GetHowOut();
            return Request.CreateResponse(HttpStatusCode.OK,
                fixtureStat);
        }

        private AdminFixtureStatViewModel GetFixtureStat(int fixtureId)
        {
            return _fixtureServices.GetAdminFixtureStatViewModel(fixtureId);
        }
    }
}
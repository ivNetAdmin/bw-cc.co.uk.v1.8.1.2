
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using NHibernate.Mapping;
using Orchard;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class AdminTeamController : ApiController
    {
        private readonly IFixtureServices _fixtureServices;
        private readonly IOrchardServices _orchardServices;        
        private readonly IPlayerServices _playerServices;

        public AdminTeamController(IFixtureServices fixtureServices, IOrchardServices orchardServices,
            IPlayerServices playerServices)
        {
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
                GetTeamAdminViewModel());
        }

        private TeamAdminViewModel GetTeamAdminViewModel()
        {
            return new TeamAdminViewModel
            {
                AdminFixtureViewModel = GetFixtures(),
                Players = GetPlayers()
            };
        }

        private AdminFixtureViewModel GetFixtures()
        {
            return _fixtureServices.GetAdminFixtureViewModel();
        }

        private List<PlayerViewModel> GetPlayers()
        {
            return _playerServices.GetActivePlayers();
        }
    }
}
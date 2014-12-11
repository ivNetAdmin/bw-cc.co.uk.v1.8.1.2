
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

            return GetFixtures();          
        }

        private HttpResponseMessage GetFixtures()
        {
            var editFixtureViewModel = new EditFixtureViewModel();
            var fixtureList = _fixtureServices.GetAll();
            var teamList = _configurationServices.GetTeams();
            var opponentList = _configurationServices.GetOpponents();
            var fixtureTypeList = _configurationServices.GetFixtureTypes();


            editFixtureViewModel.Fixtures = fixtureList.Select(fixture => MapperHelper.Map(new FixtureViewModel(), fixture)).ToList();
            editFixtureViewModel.Fixtures.Insert(0, new FixtureViewModel());


            editFixtureViewModel.Teams = (from listItem in teamList
                                                   let listItemViewModel = new TeamViewModel()
                                                   select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            editFixtureViewModel.Opponents = (from listItem in opponentList
                                              let listItemViewModel = new OpponentViewModel()
                                          select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            editFixtureViewModel.FixtureTypes = (from listItem in fixtureTypeList
                                                 let listItemViewModel = new FixtureTypeViewModel()
                                              select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            return Request.CreateResponse(HttpStatusCode.OK,
                editFixtureViewModel);
        }
    }
}
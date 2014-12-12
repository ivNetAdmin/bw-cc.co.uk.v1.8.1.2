
using System;
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

            return Request.CreateResponse(HttpStatusCode.OK,
                GetFixtures());
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, FixtureViewModel item)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            item.Id = id;

            _fixtureServices.SaveFixture(item);

            try
            {
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

        private EditFixtureViewModel GetFixtures()
        {
            var editFixtureViewModel = new EditFixtureViewModel();
            // need to map entity within session becase of lazy-loading
            editFixtureViewModel.Fixtures = _fixtureServices.GetAll();
            var fixtureList = _fixtureServices.GetAll();
            var teamList = _configurationServices.GetTeams();
            var opponentList = _configurationServices.GetOpponents();
            var locationList = _configurationServices.GetLocations();
            var fixtureTypeList = _configurationServices.GetFixtureTypes();
            
            editFixtureViewModel.Fixtures.Insert(0, new FixtureViewModel());


            editFixtureViewModel.Teams = (from listItem in teamList
                                                   let listItemViewModel = new TeamViewModel()
                                                   select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            editFixtureViewModel.Opponents = (from listItem in opponentList
                                              let listItemViewModel = new OpponentViewModel()
                                          select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            editFixtureViewModel.Locations = (from listItem in locationList
                                              let listItemViewModel = new LocationViewModel()
                                              select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            editFixtureViewModel.FixtureTypes = (from listItem in fixtureTypeList
                                                 let listItemViewModel = new FixtureTypeViewModel()
                                              select MapperHelper.Map(listItemViewModel, listItem)).ToList();

            return editFixtureViewModel;
        }
    }
}
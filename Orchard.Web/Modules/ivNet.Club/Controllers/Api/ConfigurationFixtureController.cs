
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Helpers;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using NHibernate.Mapping;
using Orchard;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class ConfigurationFixtureController : ApiController
    {
        private readonly IConfigurationServices _configurationServices;
        private readonly IOrchardServices _orchardServices;

        public ConfigurationFixtureController(IConfigurationServices configurationServices,
            IOrchardServices orchardServices)
        {
            _configurationServices = configurationServices;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivConfiguration))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, GetFixture());
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

        public HttpResponseMessage Get(int id, string type)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetFixtureConfig(type, id));
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, FixtureItemConfigViewModel item)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivConfiguration))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            try
            {
                switch (item.Type)
                {
                    case "teamconfig":
                        _configurationServices.SaveTeam(id, item.Name, item.IsActive);
                        break;
                    case "opponentconfig":
                        _configurationServices.SaveOpponent(id, item.Name, item.IsActive);
                        break;
                    case "fixturetypeconfig":
                        _configurationServices.SaveFixtureType(id, item.Name, item.IsActive);
                        break;
                    case "resulttypeconfig":
                        _configurationServices.SaveResultType(id, item.Name, item.IsActive);
                        break;
                    case "howoutconfig":
                        _configurationServices.SaveHowOut(id, item.Name, item.IsActive);
                        break;
                    case "locationconfig":
                        _configurationServices.SaveLocation(id, item.Name, item.Postcode, item.Latitude, item.Longitude, item.IsActive);
                        break;

                }

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

        private FixtureConfigViewModel GetFixture()
        {
            var fixtureConfigurationViewModel = new FixtureConfigViewModel();

            var teamItemList = _configurationServices.GetTeams();
            fixtureConfigurationViewModel.Teams = (from listItem in teamItemList
                let listItemViewModel = new FixtureItemConfigViewModel()
                select MapperHelper.Map(listItemViewModel, listItem)).ToList();
            fixtureConfigurationViewModel.Teams.Insert(0, new FixtureItemConfigViewModel());

            var opponentItemList = _configurationServices.GetOpponents();
            fixtureConfigurationViewModel.Opponents = (from listItem in opponentItemList
                let listItemViewModel = new FixtureItemConfigViewModel()
                select MapperHelper.Map(listItemViewModel, listItem)).ToList();
            fixtureConfigurationViewModel.Opponents.Insert(0, new FixtureItemConfigViewModel());

            var fixtureTypeItemList = _configurationServices.GetFixtureTypes();
            fixtureConfigurationViewModel.FixtureTypes = (from listItem in fixtureTypeItemList
                let listItemViewModel = new FixtureItemConfigViewModel()
                select MapperHelper.Map(listItemViewModel, listItem)).ToList();
            fixtureConfigurationViewModel.FixtureTypes.Insert(0, new FixtureItemConfigViewModel());

            var resultTypeItemList = _configurationServices.GetResultTypes();
            fixtureConfigurationViewModel.ResultTypes = (from listItem in resultTypeItemList
                                                          let listItemViewModel = new FixtureItemConfigViewModel()
                                                          select MapperHelper.Map(listItemViewModel, listItem)).ToList();
            fixtureConfigurationViewModel.ResultTypes.Insert(0, new FixtureItemConfigViewModel());

            var howOutItemList = _configurationServices.GetHowOut();
            fixtureConfigurationViewModel.HowOut = (from listItem in howOutItemList
                let listItemViewModel = new FixtureItemConfigViewModel()
                select MapperHelper.Map(listItemViewModel, listItem)).ToList();
            fixtureConfigurationViewModel.HowOut.Insert(0, new FixtureItemConfigViewModel());

            var locationItemList = _configurationServices.GetLocations();
            fixtureConfigurationViewModel.Locations = (from listItem in locationItemList
                let listItemViewModel = new FixtureItemConfigViewModel()
                select MapperHelper.Map(listItemViewModel, listItem)).ToList();
            fixtureConfigurationViewModel.Locations.Insert(0, new FixtureItemConfigViewModel());

            return fixtureConfigurationViewModel;
        }

        private IEnumerable<FixtureItemConfigViewModel> GetFixtureConfig(string type, int id)
        {
            switch (type)
            {
                case "opponent-locations":
                    var locationList = _configurationServices.GetLocationsByOpponentId(id);

                    return (from listItem in locationList
                        let listItemViewModel = new FixtureItemConfigViewModel()
                        select MapperHelper.Map(listItemViewModel, listItem)).ToList();
            }

            return new List<FixtureItemConfigViewModel>();
        }
    }
}
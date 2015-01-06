
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Entities;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            fixtureStat.YesNo = new List<ListItemViewModel>
            {                
                new ListItemViewModel {Id = 1, Text = "Yes"}
            };

            return Request.CreateResponse(HttpStatusCode.OK,
                fixtureStat);
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, Newtonsoft.Json.Linq.JArray item)
        {
            try
            {

                var stats = item.ToObject<List<PlayerStatViewModel>>();
                _fixtureServices.SaveFixtureStats(stats);

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


        private AdminFixtureStatViewModel GetFixtureStat(int fixtureId)
        {
            return _fixtureServices.GetAdminFixtureStatViewModel(fixtureId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
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

        public HttpResponseMessage Get(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            return Request.CreateResponse(HttpStatusCode.OK,
                GetTeamSelectionAdminViewModel(id));
        }

        public HttpResponseMessage Get(int currentPage, string orderBy, bool orderByReverse, int pageItems, string filterBy, string filterByFields)
        {

            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            return Request.CreateResponse(HttpStatusCode.OK,
                GetPaginatedTeamAdminViewModel(currentPage, orderBy, orderByReverse, pageItems, filterBy, filterByFields));

        }     

        [HttpPut]
        public HttpResponseMessage Put(int id, AdminTeamSelectionViewModel teamSelection)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);
        
            try
            {
                _fixtureServices.SaveTeamSelection(id, teamSelection);

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

        private TeamAdminViewModel GetTeamAdminViewModel()
        {
            return new TeamAdminViewModel
            {
                AdminFixtureViewModel = GetFixtures(),
                Players = GetPlayers()
            };
        }

        private TeamAdminViewModel GetPaginatedTeamAdminViewModel(int currentPage, string orderBy, bool orderByReverse, int pageItems, string filterBy, string filterByFields)
        {
            List<PlayerViewModel> playerList;

            if (HttpContext.Current.Session["PlayerList"] == null)
            {
                playerList = GetPlayers();
                HttpContext.Current.Session["PlayerList"] = playerList;
            }
            else
            {
                playerList = (List<PlayerViewModel>) HttpContext.Current.Session["PlayerList"];
            }

            return new TeamAdminViewModel
            {
                AdminFixtureViewModel =
                    GetPaginatedFixtures(currentPage, orderBy, orderByReverse, pageItems, filterBy, filterByFields),
                Players = playerList               
            };
        }

        private TeamSelectionAdminViewModel GetTeamSelectionAdminViewModel(int id)
        {
            return _fixtureServices.GetTeamSelectionAdminViewModel(id);
        }

        private AdminFixtureViewModel GetFixtures()
        {
            return _fixtureServices.GetAdminFixtureViewModel();
        }

        private AdminFixtureViewModel GetPaginatedFixtures(int currentPage, string orderBy, bool orderByReverse, int pageItems, string filterBy, string filterByFields)
        {
            return _fixtureServices.GetAdminFixtureViewModel(currentPage, orderBy, orderByReverse, pageItems, filterBy, filterByFields);
        }


        private List<PlayerViewModel> GetPlayers()
        {
            return _playerServices.GetActivePlayers();
        }
    }
}
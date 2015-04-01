
using System.Collections.Generic;
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
    public class BwccDataLoadController : ApiController
    {
        private readonly IBwccDataServices _bwccDataServices;
        private readonly IOrchardServices _orchardServices;
        private readonly IMemberServices _memberServices;        
        private readonly IPlayerServices _playerServices;
        private readonly IFixtureServices _fixtureServices;
        private readonly IConfigurationServices _configurationServices;
        

        public BwccDataLoadController(IBwccDataServices bwccDataServices, IFixtureServices fixtureServices,
            IOrchardServices orchardServices, IMemberServices memberServices,
            IPlayerServices playerServices, IConfigurationServices configurationServices)
        {
            _bwccDataServices = bwccDataServices;
            _fixtureServices = fixtureServices;
            _orchardServices = orchardServices;
            _memberServices = memberServices;
            _playerServices = playerServices;
            _configurationServices = configurationServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get(string id)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            switch (id)
            {
                case "members":
                    LoadMemberData();
                    break;
                case "activate-members":
                    ActivateData();
                    break;
                case "fixtures":
                    LoadFixtureData();
                    break;
                case "match-reports":
                    LoadMatchReportData();
                    break;
            }
            

            return Request.CreateResponse(HttpStatusCode.OK,
                "done!");
        }

        private List<EditMemberViewModel> LoadMemberData()
        {
            // get members
            var editMemberViewModelList = _bwccDataServices.GetMembers();
           
            foreach (var editMemberViewModel in editMemberViewModelList)
            {
                _memberServices.UpdateMember(editMemberViewModel);
            }            

            return editMemberViewModelList;
        }

        private List<RelatedMemberViewModel> ActivateData()
        {            

            // get members
            var relatedMemberViewModelList = _memberServices.GetAll(0);

            foreach (var relatedMemberViewModel in relatedMemberViewModelList)
            {
                _memberServices.Activate(relatedMemberViewModel.MemberId, relatedMemberViewModel.MemberType);
            }

            return relatedMemberViewModelList;
            
        }

        private void LoadFixtureData()
        {
            var configItems = new Dictionary<string, int>();

            var loadFixtureViewModelList = _bwccDataServices.GetFixtures();

            foreach (var loadFixtureViewModel in loadFixtureViewModelList)
            {

                var editFixtureViewModel = new FixtureViewModel
                {
                    Team = loadFixtureViewModel.Team,
                    Opponent = loadFixtureViewModel.Oppisition,
                    FixtureType = loadFixtureViewModel.FixtureType,
                    Date = loadFixtureViewModel.DatePlayed,
                    HomeAway = loadFixtureViewModel.Venue,
                    ResultType = string.IsNullOrEmpty(loadFixtureViewModel.ResultType) ? "Unknown" : loadFixtureViewModel.ResultType,
                    Score = loadFixtureViewModel.Score
                };

                var value = 0;
                if (configItems.TryGetValue(editFixtureViewModel.Team, out value))
                {
                    editFixtureViewModel.TeamId = value;
                }
                else
                {
                    editFixtureViewModel.TeamId = _configurationServices.SaveTeam(0, editFixtureViewModel.Team, 1);
                    configItems.Add(editFixtureViewModel.Team, editFixtureViewModel.TeamId);
                }

                if (configItems.TryGetValue(editFixtureViewModel.Opponent, out value))
                {
                    editFixtureViewModel.OpponentId = value;
                }
                else
                {
                    editFixtureViewModel.OpponentId = _configurationServices.SaveOpponent(0, editFixtureViewModel.Opponent, 1);
                    configItems.Add(editFixtureViewModel.Opponent, editFixtureViewModel.OpponentId);
                }

                if (configItems.TryGetValue(editFixtureViewModel.FixtureType, out value))
                {
                    editFixtureViewModel.FixtureTypeId = value;
                }
                else
                {
                    editFixtureViewModel.FixtureTypeId = _configurationServices.SaveFixtureType(0, editFixtureViewModel.FixtureType, 1);
                    configItems.Add(editFixtureViewModel.FixtureType, editFixtureViewModel.FixtureTypeId);
                }

                if (configItems.TryGetValue(editFixtureViewModel.ResultType, out value))
                {
                    editFixtureViewModel.ResultTypeId = value;
                }
                else
                {
                    editFixtureViewModel.ResultTypeId = _configurationServices.SaveResultType(0, editFixtureViewModel.ResultType, 1);
                    configItems.Add(editFixtureViewModel.ResultType, editFixtureViewModel.ResultTypeId);
                }

                _fixtureServices.SaveFixture(editFixtureViewModel);

                var stats = _bwccDataServices.GetFixtureStats(editFixtureViewModel.Id,
                    loadFixtureViewModel.LegacyFixtureId);

                if (stats.Count > 0)
                {
                    _fixtureServices.SaveFixtureStats(stats);
                }
            }
        }

        private void LoadMatchReportData()
        {
            var reports = _bwccDataServices.GetMatchReports();
            foreach (var matchReport in reports)
            {
                _bwccDataServices.SaveMatchReport(matchReport);
            }                       
        }
    }
}
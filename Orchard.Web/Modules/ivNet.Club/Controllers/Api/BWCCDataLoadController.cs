
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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

        public BwccDataLoadController(IBwccDataServices bwccDataServices, IFixtureServices fixtureServices,
            IOrchardServices orchardServices, IMemberServices memberServices,
            IPlayerServices playerServices)
        {
            _bwccDataServices = bwccDataServices;
            _fixtureServices = fixtureServices;
            _orchardServices = orchardServices;
            _memberServices = memberServices;
            _playerServices = playerServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageFixtures))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            LoadData();

            return Request.CreateResponse(HttpStatusCode.OK,
                "done!");
        }

        private List<EditMemberViewModel> LoadData()
        {
            // get members
            var editMemberViewModelList = _bwccDataServices.GetMembers();

            foreach (var editMemberViewModel in editMemberViewModelList)
            {
                _memberServices.UpdateMember(editMemberViewModel);
            }            

            return editMemberViewModelList;
        }
    }
}
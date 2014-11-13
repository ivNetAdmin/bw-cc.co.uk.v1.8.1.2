
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;
using Orchard;
using Orchard.Logging;
using Orchard.Security;

namespace ivNet.Club.Controllers.Api
{
    public class MyMemberController : ApiController
    {
        private readonly IMembershipService _membershipService;
        private readonly IClubMemberServices _clubMemberServices;
        private readonly IOrchardServices _orchardServices;

        public MyMemberController(IOrchardServices orchardServices, IMembershipService membershipService,
            IClubMemberServices clubMemberServices)
        {
            _clubMemberServices = clubMemberServices;
            _membershipService = membershipService;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            try
            {

                if (!_orchardServices.Authorizer.Authorize(Permissions.ivMyRegistration))
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "You are not authorized");
                
                var memberList = _clubMemberServices.GetAll();

                return Request.CreateResponse(HttpStatusCode.OK,
                    memberList);
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
    }
}
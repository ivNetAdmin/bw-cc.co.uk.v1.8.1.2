
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Enums;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class AdminMemberController : ApiController
    {

         private readonly IMemberServices _memberServices;
         private readonly IOrchardServices _orchardServices;

        public AdminMemberController(IMemberServices memberServices, IOrchardServices orchardServices)
        {
            _memberServices = memberServices;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get(string id, string type)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageMembers))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            try
            {
                switch (type)
                {
                    case "list":
                        switch (id)
                        {
                            case "activate":                              
                                return Request.CreateResponse(HttpStatusCode.OK,
                                    _memberServices.GetAll(0));

                            case "all":                             
                                return Request.CreateResponse(HttpStatusCode.OK,
                                    _memberServices.GetAll(1));
                        }

                        break;
                  
                               
                }

                throw new Exception(string.Format("Unknown paramters ,[{0}],[{1}]", id, type));
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

        public HttpResponseMessage Get(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageMembers))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            try
            {
                var memberList = _memberServices.Get(id);

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

        [HttpPut]
        public HttpResponseMessage Put(int id, EditMemberViewModel memberViewModel)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageMembers))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            try
            {
                switch (memberViewModel.Type)
                {
                    case "activate":
                        _memberServices.Activate(id, memberViewModel.MemberType);
                        break;

                    case "admin":
                        _memberServices.UpdateMember(memberViewModel);
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
    }
}
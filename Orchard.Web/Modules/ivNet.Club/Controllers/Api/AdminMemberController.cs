
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Enums;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class AdminMemberController : ApiController
    {

         private readonly IMemberServices _memberServices;

         public AdminMemberController(IMemberServices memberServices)
        {
            _memberServices = memberServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get(string id, string type)
        {
            var message = string.Empty;
            try
            {
                switch (type)
                {
                    case "list":
                        switch (id)
                        {
                            case "activate":
                                var juniorList = _memberServices.GetAll(0);

                                return Request.CreateResponse(HttpStatusCode.OK,
                                    juniorList);

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

        [HttpPut]
        public HttpResponseMessage Put(int id, EditMemberViewModel memberViewModel)
        {
            try
            {
                _memberServices.Activate(id, memberViewModel.MemberType);

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
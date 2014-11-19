
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Helpers;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class GuardianController : ApiController
    {
         private readonly IGuardianServices _guardianServices;

        public GuardianController(IGuardianServices guardianServices)
        {
            _guardianServices = guardianServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get(string id, string type)
        {
           try
            {
                switch (type)
                {
                    case "list":

                        //var juniorList = _memberServices.GetNonVetted();

                        return Request.CreateResponse(HttpStatusCode.OK);
                  
                    case "userid":                
                        return Request.CreateResponse(HttpStatusCode.OK, 
                            _guardianServices.GetByUserId(Convert.ToInt32(id)));

                    case "user":
                        return Request.CreateResponse(HttpStatusCode.OK, 
                            _guardianServices.GetRegisteredUser());

                    case "email-search":
                        return Request.CreateResponse(HttpStatusCode.OK,
                            _guardianServices.GetByEmail(id));
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
    }
}
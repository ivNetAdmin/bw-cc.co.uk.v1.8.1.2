
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class MemberController : ApiController
    {
         private readonly IMemberServices _memberServices;

         public MemberController(IMemberServices memberServices)
        {
            _memberServices = memberServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            try
            {                
                return Request.CreateResponse(HttpStatusCode.OK,
                    _memberServices.GetAll());
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
            try
            {
                var memberList = _memberServices.Get(id);

                if (memberList[0].MemberType == "Guardian")
                {
                    // get contact details
                }
                else
                {
                    // get gaurdians
                    memberList[0].Guardians = _memberServices.GetGuardians(id);

                    // get fees
                }

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

        public HttpResponseMessage Get(string id, string type)
        {
            var message = string.Empty;
            try
            {
                switch (type)
                {
                    case "email-check":
                        
                        var adult = _memberServices.GetByEmail(id);
                        if (adult != null)
                        {                            
                            message =
                                string.Format(
                                    "This eMail [{0}] is alerady being used by {1} {2}. If you are gaurdian trying to register a junior then please log into the website and use the 'My Club' page.",
                                    id, adult.Firstname, adult.Surname);                           

                        }
                        return Request.CreateResponse(HttpStatusCode.OK, message);

                    case "junior-key":
                        message = string.Empty;
                        var junior = _memberServices.GetByJuniorKey(id);
                        if (junior != null)
                        {                            
                            message =
                                string.Format(
                                    "There is already some registered with the name [{0} {1}] if you continue with registration another junior will be created using the firstname and a counter eg tom1.jones",
                                    id, adult.Firstname, adult.Surname);                           

                        }
                        return Request.CreateResponse(HttpStatusCode.OK, message);
                    case "user":
                        var user = _memberServices.AuthenticatedUser();
                        return Request.CreateResponse(HttpStatusCode.OK, user == null ? string.Empty : user.UserName);

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
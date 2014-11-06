
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
    public class RegistrationController : ApiController
    {
        private readonly IRegistrationServices _registrationServices;

        public RegistrationController(IRegistrationServices registrationServices)
        {
            _registrationServices = registrationServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            try
            {
                var juniorList = _registrationServices.GetNonVetted();
              
                return Request.CreateResponse(HttpStatusCode.OK,
                    juniorList);
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
        public HttpResponseMessage Put(int id, JuniorVettingViewModel item)
        {

            try
            {
                _registrationServices.Activate(id, item);

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
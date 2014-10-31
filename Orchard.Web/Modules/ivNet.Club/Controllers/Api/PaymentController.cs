
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;

namespace ivNet.Club.Controllers.Api
{
    public class PaymentController : ApiController
    {
        private readonly IRegistrationServices _registrationServices;

        public PaymentController(IRegistrationServices registrationServices)
        {
            _registrationServices = registrationServices;
        }

        public HttpResponseMessage Get()
        {
            var registrationIdList = _registrationServices.Get();
           
            return Request.CreateResponse(HttpStatusCode.OK,
                registrationIdList);
        }
    }
}

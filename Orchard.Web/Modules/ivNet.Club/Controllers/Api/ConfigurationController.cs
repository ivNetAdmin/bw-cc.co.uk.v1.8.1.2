
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;

namespace ivNet.Club.Controllers.Api
{
    public class ConfigurationController : ApiController
    {
         private readonly IConfigurationServices _configurationServices;

         public ConfigurationController(IConfigurationServices configurationServices)
        {
            _configurationServices = configurationServices;
        }

        public HttpResponseMessage Get()
        {
            var registrationIdList = _configurationServices.Get();
           
            return Request.CreateResponse(HttpStatusCode.OK,
                registrationIdList);
        }
    }
}
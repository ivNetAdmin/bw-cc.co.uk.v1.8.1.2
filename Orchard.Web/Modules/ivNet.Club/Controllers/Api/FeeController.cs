
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;

namespace ivNet.Club.Controllers.Api
{
    public class FeeController : ApiController
    {
        private readonly IPlayerServices _playerServices;

        public FeeController(IPlayerServices playerServices)
        {
            _playerServices = playerServices;
        }

        public HttpResponseMessage Get()
        {
            try
            {
                var playerList = _playerServices.GetCachedJuniorRegistrations();
                return Request.CreateResponse(HttpStatusCode.OK, playerList);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    ex.Message);
            }
        }
    }
}

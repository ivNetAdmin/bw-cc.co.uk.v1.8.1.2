
using System.Net;
using System.Net.Http;
using System.Web.Http;
// Rework
using ivNet.Club.Services;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class RegistrationController : ApiController
    {
        private readonly IMemberServices _memberServices;

        public RegistrationController(IMemberServices memberServices)
        {
            _memberServices = memberServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get(string id, string type)
        {
            switch (type)
            {
                case "user":
                    switch (id)
                    {
                        case "registered":
                            return Request.CreateResponse(HttpStatusCode.OK,
                        _memberServices.GetRegisteredUser());
                    }
                    break;
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        public HttpResponseMessage Get(string id, string action, string type)
        {
            var message = string.Empty;

            switch (action)
            {
                case "duplicate":
                    switch (type)
                    {
                        case "junior":
                            message = string.Empty;
                            var junior = _memberServices.GetFullName(id);
                            if (junior != null)
                            {
                                message =
                                    string.Format(
                                        "There is already someone '{4}' registered with the name {0} {1}. If you continue with registration another junior will be created using the firstname and a counter in their username eg {2}1.{3}",
                                        junior.Firstname, junior.Surname, junior.Firstname.ToLowerInvariant(),
                                        junior.Surname.ToLowerInvariant(), junior.MemberIsActive == 1 ? "active" : "inactive");

                            }
                            return Request.CreateResponse(HttpStatusCode.OK, message);

                        case "email":
                            var adult = _memberServices.GetByKey(id);
                            if (adult != null)
                            {
                                message =
                                    string.Format(
                                        "This eMail [{0}] is alerady being used by {1} {2}.",
                                        id, adult.Firstname, adult.Surname);

                            }
                            return Request.CreateResponse(HttpStatusCode.OK, message);

                    }
                    break;
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Enums;
using ivNet.Club.Helpers;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
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

        //public HttpResponseMessage Get()
        //{
        //    try
        //    {                
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //            _memberServices.GetAll(1));
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorId = Guid.NewGuid();
        //        Logger.Error(string.Format("{0}: {1}{2} [{3}]", Request.RequestUri, ex.Message,
        //            ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));

        //        return Request.CreateResponse(HttpStatusCode.InternalServerError,
        //            "An Error has occurred. Report to bp@ivnet.co.uk quoting: " + errorId);

        //    }
        //}      

        //public HttpResponseMessage Get(int id)
        //{
        //    try
        //    {
        //        var memberList = _memberServices.Get(id);

        //        //if (memberList[0].MemberType == "Guardian")
        //        //{
        //        //    // get contact details
        //        //}
        //        //else
        //        //{
        //        //    // get gaurdians
        //        //   // memberList[0].Guardians = _memberServices.GetGuardians(id);

        //        //    // get fees
        //        //}

        //        return Request.CreateResponse(HttpStatusCode.OK,
        //            memberList);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorId = Guid.NewGuid();
        //        Logger.Error(string.Format("{0}: {1}{2} [{3}]", Request.RequestUri, ex.Message,
        //            ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));

        //        return Request.CreateResponse(HttpStatusCode.InternalServerError,
        //            "An Error has occurred. Report to bp@ivnet.co.uk quoting: " + errorId);

        //    }
        //}

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

        //public HttpResponseMessage Get(string id, string type)
        //{
        //    var message = string.Empty;
        //    try
        //    {
        //        switch (type)
        //        {
        //            case "list":
        //                switch (id)
        //                {
        //                    case "activate":
        //                        var juniorList = _memberServices.GetAll(0);

        //                        return Request.CreateResponse(HttpStatusCode.OK,
        //                            juniorList);

        //                }

        //                break;
        //            case "email-check":

        //                var adult = _memberServices.GetByKey(id);
        //                if (adult != null)
        //                {
        //                    message =
        //                        string.Format(
        //                            "This eMail [{0}] is alerady being used by {1} {2}.",
        //                            id, adult.Firstname, adult.Surname);

        //                }
        //                return Request.CreateResponse(HttpStatusCode.OK, message);

        //            case "junior-key":
        //                message = string.Empty;
        //                var junior = _memberServices.GetByKey(id);
        //                if (junior != null)
        //                {
        //                    message =
        //                        string.Format(
        //                            "There is already someone '{4}' registered with the name {0} {1}. If you continue with registration another junior will be created using the firstname and a counter in their username eg {2}1.{3}",
        //                            junior.Firstname, junior.Surname, junior.Firstname.ToLowerInvariant(),
        //                            junior.Surname.ToLowerInvariant(), junior.IsActive == 1 ? "active" : "inactive");

        //                }
        //                return Request.CreateResponse(HttpStatusCode.OK, message);

        //            case "user":
        //                var user = _memberServices.AuthenticatedUser();
        //                return Request.CreateResponse(HttpStatusCode.OK, MapperHelper.Map(new UserViewModel(), user));

        //            case "userid":
        //                var member = _memberServices.GetByUserId(Convert.ToInt32(id));
        //                return Request.CreateResponse(HttpStatusCode.OK, member);
        //        }

        //        throw new Exception(string.Format("Unknown paramters ,[{0}],[{1}]", id, type));
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorId = Guid.NewGuid();
        //        Logger.Error(string.Format("{0}: {1}{2} [{3}]", Request.RequestUri, ex.Message,
        //            ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));

        //        return Request.CreateResponse(HttpStatusCode.InternalServerError,
        //            "An Error has occurred. Report to bp@ivnet.co.uk quoting: " + errorId);
        //    }
        //}
       
    }
}
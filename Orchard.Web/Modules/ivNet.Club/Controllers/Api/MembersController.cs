﻿
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Club.Services;
using Orchard.Logging;

namespace ivNet.Club.Controllers.Api
{
    public class MembersController : ApiController
    {
         private readonly IClubMemberServices _clubMemberServices;

         public MembersController(IClubMemberServices clubMemberServices)
        {
            _clubMemberServices = clubMemberServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            try
            {
                var memberList = _clubMemberServices.GetAll();

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

        public HttpResponseMessage Get(int id)
        {
            try
            {
                var memberList = _clubMemberServices.Get(id);

                if (memberList[0].MemberType == "Guardian")
                {
                    // get contact details
                }
                else
                {
                    // get gaurdians
                    memberList[0].Guardians = _clubMemberServices.GetGuardians(id);

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

        public HttpResponseMessage Get(string id,string type)
        {
            var message = string.Empty;

            try
            {
                if (type == "email")
                {
                    var adult = _clubMemberServices.GetMember(id);
                    if (adult != null)
                    {
                        message =
                            string.Format(
                                "This eMail [{0}] is alerady being used by {1} {2}. If you are gaurdian trying to register a junior then please log into the website and use the 'My Club' page.",
                                id, adult.Firstname, adult.Surname);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   message);
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
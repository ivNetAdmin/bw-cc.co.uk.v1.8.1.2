
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using ivNet.Club.Helpers;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class MemberController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMemberServices _memberServices;
        private readonly IAuthenticationService _authenticationService;

        public MemberController(IAuthenticationService authenticationService, IOrchardServices orchardServices, IMemberServices memberServices)
        {
            _orchardServices = orchardServices;
            _memberServices = memberServices;
            _authenticationService = authenticationService;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [Themed]
        public ActionResult New()
        {
            return View();
        }
        
        [Themed]
        public ActionResult NewFee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(FormCollection form)
        {
            try
            {               

                if (form["MemberCount"] == null) throw new Exception("No new member provided for registration!");

                var memberCount = Convert.ToInt32(form["MemberCount"]);
                var memberTypes = form["checkboxes"].Split(',');
                var isGuardian = memberTypes.Contains("Guardian");

                var registrationList = new List<RegistrationViewModel>();

                for (var i = 1; i <= memberCount; i++)
                {
                    var registrationViewModel = new RegistrationViewModel();

                    MapperHelper.MapNewMember(registrationViewModel.MemberViewModel, form, i, "Adult");
                    MapperHelper.MapNewContactDetail(registrationViewModel.ContactViewModel, form, i);
                    MapperHelper.MapNewAddressDetail(registrationViewModel.AddressViewModel, form, i);

                    registrationViewModel.Season = form["Season"];

                    registrationList.Add(registrationViewModel);
                }

                if (isGuardian)
                {
                    var juniorCount = Convert.ToInt32(form["JuniorCount"]);

                    // get junior club member details
                    for (var i = 1; i <= juniorCount; i++)
                    {
                        var juniorViewModel = new JuniorViewModel();
                        juniorViewModel.Dob = MapperHelper.MapNewDob(form, i);

                        MapperHelper.MapNewMember(juniorViewModel.MemberViewModel, form, i, "Junior");
                        MapperHelper.MapJuniorDetail(juniorViewModel, form, i);

                        foreach (var registrationViewModel in registrationList)
                        {
                            registrationViewModel.JuniorList.Add(juniorViewModel);
                        }
                    }

                    _memberServices.CreateGuardian(registrationList);
                }

                return RedirectToAction("NewFee");
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                Logger.Error(string.Format("{0}: {1}{2} [{3}]", ActionName, ex.Message,
                    ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));
                return View("Error", errorId);
            }
        }

        [HttpPost]
        public ActionResult ValidateCaptcha(FormCollection form)
        {
            var cientIP = Request.ServerVariables["REMOTE_ADDR"];
            var privateKey = "6LfU2fASAAAAAOFTDH3lehppnQPH2eVhbH54aQYy"; //ConfigurationManager.AppSettings["ReCaptchaPrivateKey"];

            var data = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}",
                privateKey, cientIP, form["recaptcha_challenge_field"], form["recaptcha_response_field"]);

            var byteArray = new ASCIIEncoding().GetBytes(data);

            var request = WebRequest.Create("http://www.google.com/recaptcha/api/verify");
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var response = request.GetResponse();
            var status = (((HttpWebResponse)response).StatusCode);
            dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            var responseLines = responseFromServer.Split(new string[] { "\n" }, StringSplitOptions.None);
            var success = responseLines[0].Equals("true");

            return Json(new { Success = success });
        }

        [Themed]
        public ActionResult MemberRegistration()
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            return View(currentUser);
        }       
    }
}
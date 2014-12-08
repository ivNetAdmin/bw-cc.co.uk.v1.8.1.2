
using System.Web.Mvc;
using ivNet.Club.Services;
using NHibernate.Mapping;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class SecureSiteController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
         private readonly IMemberServices _memberServices;

         public SecureSiteController(IMemberServices memberServices, IOrchardServices orchardServices)
        {
            _memberServices = memberServices;
             _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

         public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        #region membership

        [Themed]
        public ActionResult MyRegistration()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivMyRegistration, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=%2fclub%2fmember%2fmy-registration");         

            return View("Membership/MyRegistration/Index");
        }

        #endregion       
    }
}
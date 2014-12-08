
using System.Web.Mvc;
using ivNet.Club.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class AdminSiteController : BaseController
    {

        private readonly IOrchardServices _orchardServices;

        public AdminSiteController(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

         public Localizer T { get; set; }
         public ILogger Logger { get; set; }

        #region membership

        [Themed]
        public ActionResult ActivateNewMembers()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageMembers, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=%2fclub%2fadmin%2fmember%2factivate-new");

            return View("Admin/ActivateNewMembers/Index");
        }


        [Themed]
        public ActionResult ListAllMembers()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivManageMembers, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=%2fclub%2fadmin%2fmember%2flist");

            return View("Admin/ListAll/Index");
        }
        #endregion


        #region configuration

        [Themed]
        public ActionResult ConfigurationGeneral()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivConfiguration, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=%2fclub%2fadmin%2fconfiguration%2fgeneral");

            return View("Admin/Configuration/Index");
        }

        [Themed]
        public ActionResult ConfigurationTeam()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivConfiguration, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=%2fclub%2fadmin%2fconfiguration%2fteam");

            return View("Admin/Configuration/Index");
        }

        #endregion

        #region documentation
    
        [Themed]
        public ActionResult UserStories()
        {
            return View("Admin/UserStories/Index");
        }

        #endregion

    }
}
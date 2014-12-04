
using System.Web.Mvc;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class SiteController : BaseController
    {
        #region membership

        [Themed]
        public ActionResult NewRegistration()
        {
            return View("Membership/NewRegistration/Index");
        }

        #endregion
    }
}
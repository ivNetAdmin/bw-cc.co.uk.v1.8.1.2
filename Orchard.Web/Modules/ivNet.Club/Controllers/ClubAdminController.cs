
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class ClubAdminController : BaseController
    {

        public ClubAdminController()
        {
            
        Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        #region membership

        [Themed]
        public ActionResult ActivateNewMembers()
        {
            return View("ClubAdmin/ActivateNewMembers/Index");
        }

        #endregion

        
    }
}
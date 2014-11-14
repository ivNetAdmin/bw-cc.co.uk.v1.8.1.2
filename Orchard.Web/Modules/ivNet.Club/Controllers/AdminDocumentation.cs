
using System.Web.Mvc;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class AdminDocumentation : Controller
    {
        [Themed]
        public ActionResult UserStories()
        {
            return View();
        }
    }
}
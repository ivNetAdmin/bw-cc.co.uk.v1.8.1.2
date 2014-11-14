
using System.Web.Mvc;
using ivNet.Club.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class AdminMemberController : BaseController
    {
         private readonly IOrchardServices _orchardServices;
        private readonly IMemberServices _memberServices;

         public AdminMemberController(IOrchardServices orchardServices, IMemberServices memberServices)
        {
            _orchardServices = orchardServices;
            _memberServices = memberServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

         public Localizer T { get; set; }
         public ILogger Logger { get; set; }

         [Themed]
         public ActionResult Registration()
         {
             return View();
         }

         [Themed]
         public ActionResult List()
         {
             return View();
         }
    }
}


 

       

      
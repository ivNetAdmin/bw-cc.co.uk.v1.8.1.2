
using System.Web.Mvc;
using ivNet.Club.Controllers;
using ivNet.Club.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class AdminConfigurationController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IClubMemberServices _clubMemberServices;

        public AdminConfigurationController(IOrchardServices orchardServices, IClubMemberServices clubMemberServices)
        {
            _orchardServices = orchardServices;
            _clubMemberServices = clubMemberServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [Themed]
        public ActionResult Index()
        {
            return View();
        }     
    }
}
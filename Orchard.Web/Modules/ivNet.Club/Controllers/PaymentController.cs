
using System.Web.Mvc;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;

namespace ivNet.Club.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IClubMemberServices _clubMemberServices;

        public PaymentController(IOrchardServices orchardServices, IClubMemberServices clubMemberServices)
        {
            _orchardServices = orchardServices;
            _clubMemberServices = clubMemberServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Registration(PaymentRegistreationViewModel viewModel)
        {
            return View();
        }
    }
}
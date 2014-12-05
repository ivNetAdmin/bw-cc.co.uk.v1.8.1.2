
using System;
using System.Linq;
using System.Web.Mvc;
using ivNet.Club.Helpers;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard.Logging;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class SiteController : BaseController
    {
        private readonly IMemberServices _memberServices;

        public SiteController(IMemberServices memberServices)
        {
            _memberServices = memberServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        #region membership

        [Themed]
        public ActionResult NewRegistration()
        {
            return View("Membership/NewRegistration/Index");
        }

        [HttpPost]
        public ActionResult NewRegistration(FormCollection form)
        {
            try
            {
                if (form["MemberCount"] == null) throw new Exception("No new member provided for registration!");

                var memberCount = Convert.ToInt32(form["MemberCount"]);
                var memberTypes = form["checkboxes"].Split(',');
                var isGuardian = memberTypes.Contains("Guardian");

                var editMemberViewModel = new EditMemberViewModel();

                for (var i = 1; i <= memberCount; i++)
                {
                    var guardianViewModel = new _MemberViewModel();

                    MapperHelper.MapNewMember(guardianViewModel, form, "Adult", i);
                    MapperHelper.MapNewContactDetail(guardianViewModel, form, i);
                    MapperHelper.MapNewAddressDetail(guardianViewModel, form, i);

                    editMemberViewModel.Season = form["Season"];

                    editMemberViewModel.Guardians.Add(guardianViewModel);
                }

                if (isGuardian)
                {
                    var juniorCount = Convert.ToInt32(form["JuniorCount"]);

                    // get junior club member details
                    for (var i = 1; i <= juniorCount; i++)
                    {
                        var juniorViewModel = new _MemberViewModel { Dob = MapperHelper.MapNewDob(form, i) };

                        MapperHelper.MapNewMember(juniorViewModel, form, "Junior", i);
                        MapperHelper.MapJuniorDetail(juniorViewModel, form, i);

                        editMemberViewModel.Juniors.Add(juniorViewModel);

                    }

                    _memberServices.UpdateMember(editMemberViewModel);
                }

                return new RedirectResult("~/club/member/new/fee");
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                Logger.Error(string.Format("{0}: {1}{2} [{3}]", ActionName, ex.Message,
                    ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));
                return View("Error", errorId);
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;

namespace ivNet.Club.Controllers
{
    public class ClubMemberController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IClubMemberServices _clubMemberServices;

        public ClubMemberController(IOrchardServices orchardServices, IClubMemberServices clubMemberServices)
        {
            _orchardServices = orchardServices;
            _clubMemberServices = clubMemberServices;
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

        [HttpPost] 
        //public ActionResult New(NewMembershipViewModel viewModel)
        public ActionResult New(FormCollection form)
        {
            try
            {
                if(form["MemberCount"]==null) throw new Exception("No new member provded for registration!");

                var memberCount = Convert.ToInt32(form["MemberCount"]);
                var memberTypes = form["checkboxes"].Split(',');
                var isGuardian = memberTypes.Contains("Guardian");

             //   var memberList = new List<ClubMember>();
             //   var contactList = new List<ContactDetail>();

                var registrationList = new List<RegistrationViewModel>();

                for (var i = 1; i <= memberCount; i++)
                {
                    var registrationViewModel = new RegistrationViewModel();

                   // var member = new ClubMember();
                    MapperHelper.MapNewClubMember(registrationViewModel.MemberViewModel, form, i, "Adult");
                   // memberList.Add(member);

                   // var contact = new ContactDetail();
                    MapperHelper.MapNewContactDetail(registrationViewModel.ContactViewModel, form, i);
                   // contactList.Add(contact);

                    registrationList.Add(registrationViewModel);
                }

                if (isGuardian)
                {
                    var juniorCount = Convert.ToInt32(form["JuniorCount"]);
                   // var juniorList = new List<Junior>();
              
                    // get junior club member details
                    for (var i = 1; i <= juniorCount; i++)
                    {
                        var juniorViewModel = new JuniorViewModel();
                        juniorViewModel.Dob = MapperHelper.MapNewDob(form, i);

                        MapperHelper.MapNewClubMember(juniorViewModel.MemberViewModel, form, i, "Junior");
                        MapperHelper.MapJuniorDetail(juniorViewModel, form, i);                   

                        foreach (var registrationViewModel in registrationList)
                        {
                            registrationViewModel.JuniorList.Add(juniorViewModel);
                        }
                    }

                    _clubMemberServices.CreateGuardian(registrationList);
                }


                var cakes = memberCount;
                //Logger.Debug(ActionName);
               
                return View();
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                Logger.Error(string.Format("{0}: {1}{2} [{3}]", ActionName, ex.Message,
                    ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));
                return View("Error", errorId);
            }            
        }        
    }
}
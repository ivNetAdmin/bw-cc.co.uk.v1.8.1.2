using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ivNet.Club.Entities;
using ivNet.Club.Enums;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard.Security;

namespace ivNet.Club.Helpers
{
    public static class MapperHelper
    {
        #region form->models

        public static void MapNewMember(MemberViewModel viewModel, FormCollection form,
            string memberType, int counter)
        {
            if (!string.IsNullOrEmpty(form[string.Format("{0}-MemberNo-{1}", memberType, counter)]))
            {
                viewModel.MemberId =Convert.ToInt32(form[string.Format("{0}-MemberNo-{1}", memberType, counter)]);
            }
            viewModel.Surname = form[string.Format("{0}-Surname-{1}", memberType, counter)];
            viewModel.Firstname = form[string.Format("{0}-Firstname-{1}", memberType, counter)];
            viewModel.Nickname = form[string.Format("{0}-Nickname-{1}", memberType, counter)];
            if (memberType == "Junior")
            {
                viewModel.MemberKey =
                    CustomStringHelper.BuildKey(new[]
                    {                        
                        form[string.Format("{0}-Surname-{1}", memberType, counter)],
                        form[string.Format("{0}-Firstname-{1}", memberType, counter)],
                        DateTime.Parse(form[string.Format("DOB-{0}", counter)]).ToShortDateString()
                    });
            }
            else
            {
                viewModel.MemberKey =
                    CustomStringHelper.BuildKey(new[]
                    {
                        form[string.Format("Email-{0}", counter)]
                    });
            }
        }

        public static void Map(MemberViewModel viewModel, FormCollection form, string memberType)
        {
            viewModel.Surname = form[string.Format("{0}-Surname", memberType)];
            viewModel.Firstname = form[string.Format("{0}-Firstname", memberType)];
            viewModel.Nickname = form[string.Format("{0}-Nickname", memberType)];
            if (memberType == "Junior")
            {
                viewModel.MemberKey =
                    CustomStringHelper.BuildKey(new[]
                    {
                        viewModel.Surname, viewModel.Firstname,
                        DateTime.Parse(form["Dob"]).ToShortDateString()
                    });
            }
            else
            {
                viewModel.MemberKey = CustomStringHelper.BuildKey(new[] {form["Email"]});
            }
        }

        public static void Map(MemberViewModel viewModel, FormCollection form)
        {
            viewModel.Dob = DateTime.Parse(form[string.Format("Dob")]);
            viewModel.School = form[string.Format("School")];
            viewModel.Notes = form[string.Format("Notes")];            
        }

        public static void MapNewContactDetail(MemberViewModel viewModel, FormCollection form, int counter)
        {
            viewModel.Email = form[string.Format("Email-{0}", counter)];
            viewModel.Mobile = form[string.Format("Mobile-{0}", counter)];
            viewModel.OtherTelephone = form[string.Format("OtherTelephone-{0}", counter)];
            viewModel.ContactDetailKey = CustomStringHelper.BuildKey(new[] { viewModel.Email });
        }

        public static void MapNewContactDetail(MemberViewModel viewModel, FormCollection form)
        {
            viewModel.Email = form[string.Format("Email")];
            viewModel.Mobile = form[string.Format("Mobile")];
            viewModel.OtherTelephone = form[string.Format("OtherTelephone")];
            viewModel.ContactDetailKey = CustomStringHelper.BuildKey(new[] { viewModel.Email });
        }

        public static void MapNewAddressDetail(MemberViewModel viewModel, FormCollection form, int counter)
        {
            viewModel.Address = form[string.Format("Address-{0}", counter)];
            viewModel.Postcode = form[string.Format("Postcode-{0}", counter)];
            viewModel.Town = form[string.Format("Town-{0}", counter)];
            viewModel.AddressDetailKey = CustomStringHelper.BuildKey(new[] { viewModel.Address, viewModel.Postcode });
        }

        public static void MapNewAddressDetail(MemberViewModel viewModel, FormCollection form)
        {
            viewModel.Address = form[string.Format("Address")];
            viewModel.Postcode = form[string.Format("Postcode")];
            viewModel.Town = form[string.Format("Town")];
            viewModel.AddressDetailKey = CustomStringHelper.BuildKey(new[] { viewModel.Address, viewModel.Postcode });
        }


        public static DateTime MapNewDob(FormCollection form, int counter)
        {
            return DateTime.Parse(form[string.Format("DOB-{0}", counter)]);
        }

        public static void MapJuniorDetail(MemberViewModel viewModel, FormCollection form, int counter)
        {
            viewModel.School = form[string.Format("School-{0}", counter)];
            viewModel.Team = form[string.Format("Team-{0}", counter)];
            viewModel.Notes = form[string.Format("Notes-{0}", counter)];

            viewModel.BootSize = form[string.Format("Boot-{0}", counter)];
            viewModel.ShirtSize = form[string.Format("Shirt-{0}", counter)];
            viewModel.ShortSize = form[string.Format("Shorts-{0}", counter)];
        }
       
        #endregion

        #region models->entities

        public static JuniorInfo Map(JuniorInfo entity, MemberViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static Kit Map(Kit entity, MemberViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static Member Map(Member entity, MemberViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static ContactDetail Map(ContactDetail entity, MemberViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static AddressDetail Map(AddressDetail entity, MemberViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static void UpdateMap(Member entity, MemberViewModel viewModel)
        {
           entity.Nickname = viewModel.Nickname;
        }

        //public static void Map(Guardian entity, JuniorViewModel viewModel)
        //{
        //    var juniorMapped = false;
        //    foreach (var junior in entity.Juniors)
        //    {
        //        // if same junior
        //        if (junior.Member.Id == viewModel.MemberViewModel.MemberId)
        //        {
        //            junior.Dob = viewModel.Dob;                    

        //            junior.Member.Nickname = viewModel.MemberViewModel.Nickname;

        //            junior.JuniorInfo.School = viewModel.School;
        //            junior.JuniorInfo.Notes = viewModel.Notes;
                    
        //            juniorMapped = true;
        //        }
        //    }

        //    // if new junior add
        //    if (!juniorMapped)
        //    {
        //        var newJunior = new Junior {Dob = viewModel.Dob};
        //        newJunior.Init();
        //        newJunior.Player.Init();

        //        newJunior.JuniorKey =
        //            CustomStringHelper.BuildKey(new[]
        //            {newJunior.Member.Surname, newJunior.Member.Firstname, newJunior.Dob.ToShortDateString()});

        //        newJunior.Member.Surname = viewModel.MemberViewModel.Surname;
        //        newJunior.Member.Firstname = viewModel.MemberViewModel.Firstname;
        //        newJunior.Member.Nickname = viewModel.MemberViewModel.Nickname;
        //        newJunior.Member.MemberKey =
        //            CustomStringHelper.BuildKey(new[] { newJunior.Member.Firstname, newJunior.Member.Surname, newJunior.Dob.ToShortDateString()});

        //        newJunior.JuniorInfo.School = viewModel.School;
        //        newJunior.JuniorInfo.Notes = viewModel.Notes;

        //        entity.AddJunior(newJunior);
        //    }
        //}

        public static void Map(Guardian entity, RegistrationViewModel viewModel)
        {
            // member details
            if (!string.IsNullOrEmpty(viewModel.MemberViewModel.Firstname))
                entity.Member.Firstname = viewModel.MemberViewModel.Firstname;

            if (!string.IsNullOrEmpty(viewModel.MemberViewModel.Surname))
                entity.Member.Surname = viewModel.MemberViewModel.Surname;

            entity.Member.Nickname = viewModel.MemberViewModel.Nickname;

            if (!string.IsNullOrEmpty(viewModel.ContactViewModel.Email))
                entity.Member.MemberKey = CustomStringHelper.BuildKey(new[] {viewModel.ContactViewModel.Email});

            // contact details
            if (!string.IsNullOrEmpty(viewModel.ContactViewModel.Email))
            {
                entity.ContactDetail.Email = viewModel.ContactViewModel.Email;
                entity.ContactDetail.ContactDetailKey = CustomStringHelper.BuildKey(new[] {entity.ContactDetail.Email});
            }

            if (!string.IsNullOrEmpty(viewModel.ContactViewModel.Mobile))
                entity.ContactDetail.Mobile = viewModel.ContactViewModel.Mobile;

            entity.ContactDetail.OtherTelephone = viewModel.ContactViewModel.OtherTelephone; 

            // address details
            entity.AddressDetail.Address = viewModel.AddressViewModel.Address;
            entity.AddressDetail.Postcode = viewModel.AddressViewModel.Postcode;
            entity.AddressDetail.Town = viewModel.AddressViewModel.Town;

            entity.AddressDetail.AddressDetailKey =
                CustomStringHelper.BuildKey(new[] {entity.AddressDetail.Address, entity.AddressDetail.Postcode});

            entity.GuardianKey = entity.Member.MemberKey;
        }

        //public static Member Map(Member entity, MemberViewModel viewModel)
        //{
        //    return Mapper.Map(viewModel, entity);
        //}

        public static ContactDetail Map(ContactDetail entity, ContactViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static AddressDetail Map(AddressDetail entity, AddressViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static JuniorInfo Map(JuniorInfo entity, JuniorViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static Kit Map(Kit entity, JuniorViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static ConfigurationItem Map(ConfigurationItem entity, ConfigurationItemViewModel viewModel)
        {
            entity.IsActive = viewModel.IsActive;
            return Mapper.Map(viewModel, entity);
        }

        public static Member Map(Member entity, FormCollection viewModel)
        {
            entity.Surname = viewModel["NewJuniorSurname"];
            entity.Firstname = viewModel["NewJuniorFirstname"];
            entity.Nickname = viewModel["NewJuniorNickName"];
            entity.MemberKey =
                    CustomStringHelper.BuildKey(new[]
                    {                        
                        entity.Surname,
                        entity.Firstname,
                        entity.Surname = viewModel["NewJuniorDob"]
                    });
            return entity;
        }

        public static JuniorInfo Map(JuniorInfo entity, FormCollection viewModel)
        {
            entity.Notes = viewModel["NewJuniorNotes"];
            entity.School = viewModel["NewJuniorSchool"];
            return entity;
        }

        public static Junior Map(Junior entity, FormCollection viewModel)
        {
            entity.Dob = DateTime.Parse(viewModel["NewJuniorDob"]);
            return entity;
        }

        #endregion

        #region entities->models

        public static LocationViewModel Map(LocationViewModel viewModel, Location entity)
        {
            return new LocationViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Postcode = entity.Postcode,
                Longitude = entity.Longitude,
                Latitude = entity.Latitude
            };
        }

        public static TeamViewModel Map(TeamViewModel viewModel, Team entity)
        {
            return new TeamViewModel {Id = entity.Id, Name = entity.Name};
        }

        public static OpponentViewModel Map(OpponentViewModel viewModel, Opponent entity)
        {
            return new OpponentViewModel { Id = entity.Id, Name = entity.Name };
        }

        public static FixtureTypeViewModel Map(FixtureTypeViewModel viewModel, FixtureType entity)
        {
            return new FixtureTypeViewModel { Id = entity.Id, Name = entity.Name };
        }

        public static FixtureViewModel Map(FixtureViewModel viewModel, Fixture entity)
        {
            return new FixtureViewModel
            {
                Id = entity.Id,
                Date = entity.Date,
                HomeAway = entity.HomeAway,
                Team = entity.Team.Name,
                TeamId = entity.Team.Id,
                Opponent = entity.Opponent.Name,
                OpponentId = entity.Opponent.Id,
                FixtureType = entity.FixtureType.Name,
                FixtureTypeId = entity.FixtureType.Id,
                Location = entity.Location.Postcode,
                LocationId = entity.Location.Id
            };
        }

        public static PlayerViewModel Map(PlayerViewModel viewModel, Junior entity)
        {
            return new PlayerViewModel
            {
                  MemberId = entity.Member.Id,
                  PlayerNumber = entity.Player.Number,
                  Name = string.Format("{0}, {1}", entity.Member.Surname,entity.Member.Firstname),
                  Nickname = entity.Member.Nickname,
                  Dob = entity.Dob
            };
        }

        public static PlayerViewModel Map(PlayerViewModel viewModel, Senior entity)
        {
            return new PlayerViewModel
            {
                MemberId = entity.Member.Id,
                PlayerNumber = entity.Player.Number,
                Name = string.Format("{0}, {1}", entity.Member.Surname, entity.Member.Firstname),
                Nickname = entity.Member.Nickname
            };
        }

        public static FixtureItemConfigViewModel Map(FixtureItemConfigViewModel viewModel, Team entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static FixtureItemConfigViewModel Map(FixtureItemConfigViewModel viewModel, Opponent entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static FixtureItemConfigViewModel Map(FixtureItemConfigViewModel viewModel, FixtureType entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static FixtureItemConfigViewModel Map(FixtureItemConfigViewModel viewModel, Location entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static MemberViewModel Map(MemberViewModel viewModel, AddressDetail entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static MemberViewModel Map(MemberViewModel viewModel, ContactDetail entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static MemberViewModel Map(MemberViewModel viewModel, Member entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static MemberViewModel Map(MemberViewModel viewModel, JuniorInfo entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static RelatedMemberViewModel Map(RelatedMemberViewModel viewModel, Guardian entity)
        {
            viewModel = Mapper.Map(entity.Member, viewModel);
            viewModel.MemberType = (int)MemberType.Guardian;
            foreach (var junior in entity.Juniors)
            {
                viewModel.RelatedMembeList.Add(string.Format("{0} {1}", junior.Member.Firstname, junior.Member.Surname));
            }
            viewModel.IsActive = entity.IsActive;
            return viewModel;
        }

        public static RelatedMemberViewModel Map(RelatedMemberViewModel viewModel, Junior entity)
        {
            viewModel = Mapper.Map(entity.Member, viewModel);
            viewModel.MemberType = (int)MemberType.Junior;
            viewModel.Dob = entity.Dob;
            foreach (var guardian in entity.Guardians)
            {
                viewModel.RelatedMembeList.Add(string.Format("{0} {1}", guardian.Member.Firstname, guardian.Member.Surname));
            }
            viewModel.IsActive = entity.IsActive;
            return viewModel;
        }

        public static JuniorDetailViewModel Map(JuniorDetailViewModel viewModel, Junior entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static MemberDetailViewModel Map(MemberDetailViewModel viewModel, Member entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static MemberDetailViewModel Map(MemberDetailViewModel viewModel, AddressDetail entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static MemberDetailViewModel Map(MemberDetailViewModel viewModel, ContactDetail entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static AddressViewModel Map(AddressViewModel viewModel, AddressDetail entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static ContactViewModel Map(ContactViewModel viewModel, ContactDetail entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        //public static MemberViewModel Map(MemberViewModel viewModel, Member entity)
        //{
        //    return Mapper.Map(entity, viewModel);
        //}

        public static JuniorViewModel Map(JuniorViewModel viewModel, Junior entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static UserViewModel Map(UserViewModel viewModel, IUser entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static GuardianViewModel Map(GuardianViewModel viewModel, Guardian entity)
        {
            viewModel.GuardianId = entity.Id;
            viewModel.Surname = entity.Member.Surname;
            viewModel.Firstname = entity.Member.Firstname;
            viewModel.Email = entity.ContactDetail.Email;
            viewModel.Mobile = entity.ContactDetail.Mobile;
            viewModel.OtherTelephone = entity.ContactDetail.OtherTelephone;
            viewModel.Address = entity.AddressDetail.Address;
            viewModel.Town = entity.AddressDetail.Town;
            viewModel.Postcode = entity.AddressDetail.Postcode;
            viewModel.IsActive = entity.IsActive;
            return viewModel;
        }

        public static JuniorVettingViewModel Map(IConfigurationServices configurationServices, JuniorVettingViewModel viewModel, Junior entity)
        {
            viewModel.Surname = entity.Member.Surname;
            viewModel.Firstname = entity.Member.Firstname;
            viewModel.Dob = entity.Dob;
            viewModel.AgeGroup = string.Format("U{0}",configurationServices.GetJuniorYear(entity.Dob));
            viewModel.JuniorId = entity.Id;
            viewModel.IsVetted = entity.IsVetted;

            foreach (var guardian in entity.Guardians)
            {
                viewModel.Guardians.Add(new JuniorGuardianViewModel
                {   
                    Surname = guardian.Member.Surname,
                    Firstname = guardian.Member.Firstname,
                    Email = guardian.ContactDetail.Email,
                    Telephone = string.Format("{0}{1}",
                        guardian.ContactDetail.Mobile,
                        string.IsNullOrEmpty(guardian.ContactDetail.OtherTelephone)
                            ? string.Empty
                            : string.Format(" ,{0}", guardian.ContactDetail.OtherTelephone))
                });
            }
            return viewModel;
        }

        public static JuniorNewRegistrationFeeViewModel Map(JuniorNewRegistrationFeeViewModel viewModel, Junior entity)
        {
            viewModel.Surname = entity.Member.Surname;
            viewModel.Firstname = entity.Member.Firstname;
            viewModel.Dob = entity.Dob;
            return viewModel;
        }

        public static JuniorNewRegistrationFeeViewModel Map(JuniorNewRegistrationFeeViewModel viewModel, Junior entity , string currentSeason)
        {
            // get fee for this current season
            foreach (var fee in entity.Player.Fees.Where(fee => fee.Season == currentSeason))
            {
                viewModel.Fee = fee.Amount;
            }

            viewModel.Surname = entity.Member.Surname;
            viewModel.Firstname = entity.Member.Firstname;
            viewModel.Dob = entity.Dob;
            viewModel.Season = currentSeason;
            
            return viewModel;
        }

        public static ConfigurationItemViewModel Map(ConfigurationItemViewModel viewModel, ConfigurationItem entity)
        {
            viewModel.IsActive = entity.IsActive;
            return Mapper.Map(entity, viewModel);
        }

        //public static MemberViewModel Map(IConfigurationServices configurationServices, MemberViewModel viewModel, Junior entity)
        //{
        //    var currentSeason = configurationServices.GetCurrentSeason();

        //    //// get fee for this current season
        //    //foreach (var fee in entity.Player.Fees.Where(fee => fee.Season == currentSeason))
        //    //{
        //    //    viewModel.Fee = string.Format("{0} - {1}", currentSeason, fee.Amount);
        //    //}

        //    viewModel.MemberId = entity.Member.Id;
        //    viewModel.Surname = entity.Member.Surname;
        //    viewModel.Firstname = entity.Member.Firstname;
        //    viewModel.Dob = entity.Dob;
        //    viewModel.MemberType = string.Format("U{0}", configurationServices.GetJuniorYear(entity.Dob));
        //    viewModel.IsActive = entity.IsActive;
        //    return viewModel;
        //}

        //public static MemberViewModel Map(MemberViewModel viewModel, Guardian entity)
        //{
        //    viewModel.MemberId = entity.Member.Id;
        //    viewModel.Surname = entity.Member.Surname;
        //    viewModel.Firstname = entity.Member.Firstname;

        //    viewModel.MemberType = "Guardian";
        //    viewModel.IsActive = entity.IsActive;
        //    return viewModel;
        //}
        #endregion       
    }
}
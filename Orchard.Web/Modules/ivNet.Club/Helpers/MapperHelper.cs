using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ivNet.Club.Entities;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using NHibernate.Transform;
using Orchard.Security;

namespace ivNet.Club.Helpers
{
    public static class MapperHelper
    {
        #region form->models

        public static void MapNewMember(MemberViewModel viewModel, FormCollection form, int counter,
            string memberType)
        {
            viewModel.Surname = form[string.Format("{0}-Surname-{1}", memberType, counter)];
            viewModel.Firstname = form[string.Format("{0}-Firstname-{1}", memberType, counter)];
            viewModel.NickName = form[string.Format("{0}-NickName-{1}", memberType, counter)];
            if (memberType == "Junior")
            {
                viewModel.MemberKey =
                    CustomStringHelper.BuildKey(new[]
                    {                        
                        form[string.Format("{0}-Surname-{1}", memberType, counter)],
                        form[string.Format("{0}-Firstname-{1}", memberType, counter)]
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

        public static void MapNewContactDetail(ContactViewModel viewModel, FormCollection form, int counter)
        {
            viewModel.Email = form[string.Format("Email-{0}", counter)];
            viewModel.Mobile = form[string.Format("Mobile-{0}", counter)];
            viewModel.OtherTelephone = form[string.Format("OtherTelephone-{0}", counter)];
            viewModel.ContactDetailKey = CustomStringHelper.BuildKey(new[] { viewModel.Email });
        }


        public static void MapNewAddressDetail(AddressViewModel viewModel, FormCollection form, int counter)
        {
            viewModel.Address = form[string.Format("Address-{0}", counter)];
            viewModel.Postcode = form[string.Format("Postcode-{0}", counter)];
            viewModel.Town = form[string.Format("Town-{0}", counter)];
            viewModel.AddressDetailKey = CustomStringHelper.BuildKey(new[] { viewModel.Address, viewModel.Postcode });
        }


        public static DateTime MapNewDob(FormCollection form, int counter)
        {
            return DateTime.Parse(form[string.Format("DOB-{0}", counter)]);
        }

        public static void MapJuniorDetail(JuniorViewModel viewModel, FormCollection form, int counter)
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

        public static Member Map(Member entity, MemberViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

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

        #endregion

        #region entities->models

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

        public static MemberViewModel Map(MemberViewModel viewModel, Member entity)
        {
            return Mapper.Map(entity, viewModel);
        }

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

        public static JuniorRegistrationViewModel Map(JuniorRegistrationViewModel viewModel, Junior entity)
        {
            viewModel.Surname = entity.Member.Surname;
            viewModel.Firstname = entity.Member.Firstname;
            viewModel.Dob = entity.Dob;
            return viewModel;
        }

        public static JuniorRegistrationViewModel Map(JuniorRegistrationViewModel viewModel, Junior entity , string currentSeason)
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

        public static MemberViewModel Map(IConfigurationServices configurationServices, MemberViewModel viewModel, Junior entity)
        {
            var currentSeason = configurationServices.GetCurrentSeason();

            // get fee for this current season
            foreach (var fee in entity.Player.Fees.Where(fee => fee.Season == currentSeason))
            {
                viewModel.Fee = string.Format("{0} - {1}", currentSeason, fee.Amount);
            }

            viewModel.MemberId = entity.Member.Id;
            viewModel.Surname = entity.Member.Surname;
            viewModel.Firstname = entity.Member.Firstname;
            viewModel.Dob = entity.Dob;
            viewModel.MemberType = string.Format("U{0}", configurationServices.GetJuniorYear(entity.Dob));
            viewModel.IsActive = entity.IsActive;
            return viewModel;
        }

        public static MemberViewModel Map(MemberViewModel viewModel, Guardian entity)
        {
            viewModel.MemberId = entity.Member.Id;
            viewModel.Surname = entity.Member.Surname;
            viewModel.Firstname = entity.Member.Firstname;

            viewModel.MemberType = "Guardian";
            viewModel.IsActive = entity.IsActive;
            return viewModel;
        }
        #endregion
    }
}
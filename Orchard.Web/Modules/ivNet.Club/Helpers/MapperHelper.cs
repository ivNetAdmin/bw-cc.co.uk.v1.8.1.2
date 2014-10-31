using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ivNet.Club.Entities;
using ivNet.Club.ViewModel;
using NHibernate.Transform;

namespace ivNet.Club.Helpers
{
    public static class MapperHelper
    {
        #region form->models

        public static void MapNewClubMember(MemberViewModel viewModel, FormCollection form, int counter,
            string memberType)
        {
            viewModel.Surname = form[string.Format("{0}-Surname-{1}", memberType, counter)];
            viewModel.Firstname = form[string.Format("{0}-Firstname-{1}", memberType, counter)];
            viewModel.NickName = form[string.Format("{0}-NickName-{1}", memberType, counter)];
            if (memberType == "Junior")
            {
                viewModel.ClubMemberKey =
                    CustomStringHelper.BuildKey(new[]
                    {
                        // primary (first) guardian
                        form[string.Format("Email-{0}", 1)],
                        form[string.Format("{0}-Firstname-{1}", memberType, counter)]
                    });
            }
            else
            {
                viewModel.ClubMemberKey =
                    CustomStringHelper.BuildKey(new[]
                    {
                        form[string.Format("Email-{0}", counter)]
                    });
            }
        }     

        public static void MapNewContactDetail(ContactViewModel viewModel, FormCollection form, int counter)
        {
            viewModel.Address = form[string.Format("Address-{0}", counter)];
            viewModel.Postcode = form[string.Format("Postcode-{0}", counter)];
            viewModel.Town = form[string.Format("Town-{0}", counter)];
            viewModel.Email = form[string.Format("Email-{0}", counter)];
            viewModel.Mobile = form[string.Format("Mobile-{0}", counter)];
            viewModel.OtherTelephone = form[string.Format("OtherTelephone-{0}", counter)];
            viewModel.ContactDetailKey = CustomStringHelper.BuildKey(new[] { viewModel.Address,viewModel.Postcode });
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

        public static ClubMember Map(ClubMember entity, MemberViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }

        public static ContactDetail Map(ContactDetail entity, ContactViewModel viewModel)
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

        #endregion

        #region entities->models

        public static JuniorRegistrationViewModel Map(JuniorRegistrationViewModel viewModel, Junior entity)
        {
            viewModel.Surname = entity.ClubMember.Surname;
            viewModel.Firstname = entity.ClubMember.Firstname;
            viewModel.Dob = entity.Dob;
            return viewModel;
        }

        #endregion
    }
}
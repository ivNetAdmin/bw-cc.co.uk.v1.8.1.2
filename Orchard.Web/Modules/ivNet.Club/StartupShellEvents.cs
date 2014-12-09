﻿
using AutoMapper;
using ivNet.Club.Entities;
using ivNet.Club.ViewModel;
using Orchard.Environment;
using Orchard.Security;

namespace ivNet.Club
{
    public class StartupShellEvents : IOrchardShellEvents
    {
        public void Activated()
        {

            #region models->entities
            Mapper.CreateMap<MemberViewModel, Member>();
            Mapper.CreateMap<MemberViewModel, ContactDetail>();
            Mapper.CreateMap<MemberViewModel, AddressDetail>();

            Mapper.CreateMap<MemberViewModel, JuniorInfo>();
            Mapper.CreateMap<MemberViewModel, Kit>();

            //Mapper.CreateMap<MemberViewModel, Member>();
            Mapper.CreateMap<ContactViewModel, ContactDetail>();
            Mapper.CreateMap<AddressViewModel, AddressDetail>();

            Mapper.CreateMap<JuniorViewModel, JuniorInfo>();
            Mapper.CreateMap<JuniorViewModel, Kit>();

            Mapper.CreateMap<ConfigurationItemViewModel, ConfigurationItem>();     

            #endregion

            #region entities->models

            Mapper.CreateMap<Team, ListItemViewModel>()
                .ForMember(v => v.Value, m => m.MapFrom(e => e.Id))
                .ForMember(v => v.Text, m => m.MapFrom(e => e.Name));         

            Mapper.CreateMap<JuniorInfo, MemberViewModel>();                

            Mapper.CreateMap<AddressDetail, MemberViewModel>();                

            Mapper.CreateMap<ContactDetail, MemberViewModel>();                

            Mapper.CreateMap<Member, MemberViewModel>()
                .ForMember(v => v.MemberId, m => m.MapFrom(e=>e.Id));         
            
            Mapper.CreateMap<Member, RelatedMemberViewModel>()
                .ForMember(v => v.MemberId, m => m.MapFrom(e => e.Id));      

            Mapper.CreateMap<Junior, JuniorDetailViewModel>()
                .ForMember(v => v.MemberId, m => m.MapFrom(e => e.Member.Id))
                .ForMember(v => v.MemberKey, m => m.MapFrom(e => e.Member.MemberKey))
                .ForMember(v => v.Firstname, m => m.MapFrom(e => e.Member.Firstname))
                .ForMember(v => v.Surname, m => m.MapFrom(e => e.Member.Surname))
                .ForMember(v => v.Nickname, m => m.MapFrom(e => e.Member.Nickname))
                .ForMember(v => v.School, m => m.MapFrom(e => e.JuniorInfo.School))
                .ForMember(v => v.Notes, m => m.MapFrom(e => e.JuniorInfo.Notes));

            Mapper.CreateMap<Member, MemberDetailViewModel>()
               .ForMember(v => v.MemberId, m => m.MapFrom(e => e.Id));
            Mapper.CreateMap<AddressDetail, MemberDetailViewModel>();
            Mapper.CreateMap<ContactDetail, MemberDetailViewModel>();

            Mapper.CreateMap<AddressDetail, AddressViewModel>();
            Mapper.CreateMap<ContactDetail, ContactViewModel>();

            Mapper.CreateMap<ConfigurationItem, ConfigurationItemViewModel>();
            //Mapper.CreateMap<Member, MemberViewModel>()
            //    .ForMember(v => v.MemberId, m => m.MapFrom(e => e.Id));
            Mapper.CreateMap<Junior, JuniorViewModel>();
               // .ForMember(v => v., m => m.MapFrom(e => e.Id));
            Mapper.CreateMap<IUser, UserViewModel>();       

            #endregion

            //Mapper.CreateMap<Member, Member>();
            //Mapper.CreateMap<ContactDetail, ContactDetail>();

            //Mapper.CreateMap<NewMembershipViewModel, ContactDetail>();
            //Mapper.CreateMap<NewMembershipViewModel, Member>();
        }

        public void Terminating()
        {
            
        }
    }
}

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
            Mapper.CreateMap<_MemberViewModel, Member>();
            Mapper.CreateMap<_MemberViewModel, ContactDetail>();
            Mapper.CreateMap<_MemberViewModel, AddressDetail>();

            Mapper.CreateMap<_MemberViewModel, JuniorInfo>();
            Mapper.CreateMap<_MemberViewModel, Kit>();

            Mapper.CreateMap<MemberViewModel, Member>();
            Mapper.CreateMap<ContactViewModel, ContactDetail>();
            Mapper.CreateMap<AddressViewModel, AddressDetail>();

            Mapper.CreateMap<JuniorViewModel, JuniorInfo>();
            Mapper.CreateMap<JuniorViewModel, Kit>();

            Mapper.CreateMap<ConfigurationItemViewModel, ConfigurationItem>();     

            #endregion

            #region entities->models

            Mapper.CreateMap<JuniorInfo, _MemberViewModel>();                

            Mapper.CreateMap<AddressDetail, _MemberViewModel>();                

            Mapper.CreateMap<ContactDetail, _MemberViewModel>();                

            Mapper.CreateMap<Member, _MemberViewModel>()
                .ForMember(v => v.MemberId, m => m.MapFrom(e=>e.Id));         
            
            Mapper.CreateMap<Member, RelatedMemberViewModel>();             

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
            Mapper.CreateMap<Member, MemberViewModel>()
                .ForMember(v => v.MemberId, m => m.MapFrom(e => e.Id));
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
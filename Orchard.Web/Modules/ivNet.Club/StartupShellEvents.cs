
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
            Mapper.CreateMap<ContactViewModel, ContactDetail>();
            Mapper.CreateMap<AddressViewModel, AddressDetail>();

            Mapper.CreateMap<JuniorViewModel, JuniorInfo>();
            Mapper.CreateMap<JuniorViewModel, Kit>();

            Mapper.CreateMap<ConfigurationItemViewModel, ConfigurationItem>();     

            #endregion

            #region entities->models

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
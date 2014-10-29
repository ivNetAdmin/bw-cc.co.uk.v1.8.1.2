
using AutoMapper;
using ivNet.Club.Entities;
using ivNet.Club.ViewModel;
using Orchard.Environment;

namespace ivNet.Club
{
    public class StartupShellEvents : IOrchardShellEvents
    {
        public void Activated()
        {

            Mapper.CreateMap<MemberViewModel, ClubMember>();
            Mapper.CreateMap<ContactViewModel, ContactDetail>();

            Mapper.CreateMap<JuniorViewModel, JuniorInfo>();
            Mapper.CreateMap<JuniorViewModel, Kit>();

            

            //Mapper.CreateMap<ClubMember, ClubMember>();
            //Mapper.CreateMap<ContactDetail, ContactDetail>();

            //Mapper.CreateMap<NewMembershipViewModel, ContactDetail>();
            //Mapper.CreateMap<NewMembershipViewModel, ClubMember>();
        }

        public void Terminating()
        {
            
        }
    }
}
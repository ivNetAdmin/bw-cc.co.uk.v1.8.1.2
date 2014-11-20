
using System.Linq;
using ivNet.Club.Entities;
using ivNet.Club.Helpers;
using ivNet.Club.ViewModel;
using Orchard;
using Orchard.Security;

namespace ivNet.Club.Services
{
    public interface IGuardianServices : IDependency
    {
        RegistrationViewModel GetByUserId(int id);
        RegisteredGuardianViewModel GetRegisteredUser();
        GuardianViewModel GetByEmail(string email);
    }

    public class GuardianServices : IGuardianServices
    {
        private readonly IAuthenticationService _authenticationService;

        public GuardianServices(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public RegistrationViewModel GetByUserId(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var registrationViewModel = new RegistrationViewModel();

                var guardian = session.CreateCriteria(typeof(Guardian))
                    .List<Guardian>().FirstOrDefault(x => x.Member.UserId.Equals(id));
                if (guardian != null)
                {
                    MapperHelper.Map(registrationViewModel.MemberViewModel, guardian.Member);
                    foreach (var junior in guardian.Juniors)
                    {
                        var juniorViewModel = new JuniorViewModel();
                        juniorViewModel = MapperHelper.Map(juniorViewModel, junior);
                        registrationViewModel.JuniorList.Add(juniorViewModel);
                    }
                }

                return registrationViewModel;
            }
        }

        public RegisteredGuardianViewModel GetRegisteredUser()
        {
            var user = _authenticationService.GetAuthenticatedUser();

            using (var session = NHibernateHelper.OpenSession())
            {
                var registrationViewModel = new RegisteredGuardianViewModel();

                var guardian = session.CreateCriteria(typeof(Guardian))
                    .List<Guardian>().FirstOrDefault(x => x.Member.UserId.Equals(user.Id));
                if (guardian != null)
                {
                    var memberDetailViewModel = new MemberDetailViewModel();
                    MapperHelper.Map(memberDetailViewModel, guardian.Member);
                    MapperHelper.Map(memberDetailViewModel, guardian.AddressDetail);
                    MapperHelper.Map(memberDetailViewModel, guardian.ContactDetail);
                    registrationViewModel.MemberDetails.Add(memberDetailViewModel);

                    foreach (var junior in guardian.Juniors)
                    {
                        var juniorDetailViewModel = new JuniorDetailViewModel();
                        MapperHelper.Map(juniorDetailViewModel, junior);
                        registrationViewModel.JuniorList.Add(juniorDetailViewModel);
                    }
                }

                return registrationViewModel;
            }
        }

        public GuardianViewModel GetByEmail(string email)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var guardianViewModel = new GuardianViewModel();

                var guardian = session.CreateCriteria(typeof (Guardian))
                    .List<Guardian>().FirstOrDefault(x => x.ContactDetail.Email.Equals(email));

                return guardian == null ? null : MapperHelper.Map(guardianViewModel, guardian);
            }
        }
    }
}
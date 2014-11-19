
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
        RegistrationViewModel GetRegisteredUser();
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

        public RegistrationViewModel GetRegisteredUser()
        {
            var user = _authenticationService.GetAuthenticatedUser();

            using (var session = NHibernateHelper.OpenSession())
            {
                var registrationViewModel = new RegistrationViewModel();

                var guardian = session.CreateCriteria(typeof(Guardian))
                    .List<Guardian>().FirstOrDefault(x => x.Member.UserId.Equals(user.Id));
                if (guardian != null)
                {
                    MapperHelper.Map(registrationViewModel.MemberViewModel, guardian.Member);
                    MapperHelper.Map(registrationViewModel.AddressViewModel, guardian.AddressDetail);
                    MapperHelper.Map(registrationViewModel.ContactViewModel, guardian.ContactDetail);

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
    }
}
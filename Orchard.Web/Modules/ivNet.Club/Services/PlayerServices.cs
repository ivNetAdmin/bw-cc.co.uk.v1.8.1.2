

using ivNet.Club.Helpers;
using Orchard;
using Orchard.Data;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;

namespace ivNet.Club.Services
{

    public interface IPlayerServices : IDependency
    {

    }

    public class PlayerServices : BaseService, IPlayerServices
    {
        public PlayerServices(
            IMembershipService membershipService,
            IAuthenticationService authenticationService,
            IRoleService roleService,
            IRepository<UserRolesPartRecord> userRolesRepository)
            : base(membershipService, authenticationService, roleService, userRolesRepository)
        {

        //      public List<JuniorRegistrationViewModel> Get()
        //{
        //    var currentSeason = _configurationServices.GetCurrentSeason();

        //    using (var session = NHibernateHelper.OpenSession())
        //    {
        //        var memberIdList = ItemsInternal;

        //        var juniorList = session
        //            .CreateCriteria(typeof (Junior))
        //            .Add(Restrictions.In("Member.Id", memberIdList))
        //            .List<Junior>();

        //        var juniorRegistrationViewModelList =  (
        //            from junior in juniorList
        //            let juniorRegistrationViewModel = new JuniorRegistrationViewModel()
        //            select MapperHelper.Map(juniorRegistrationViewModel, junior, currentSeason)).OrderBy(vm => vm.Dob).ToList();

        //        AddFees(juniorRegistrationViewModelList);

        //        return juniorRegistrationViewModelList;
        //    }          
        //}
        }
    }
}
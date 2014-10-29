
using Orchard.Data.Migration;
using Orchard.Roles.Services;

namespace ivNet.Club
{
    
    public class Migrations: DataMigrationImpl
    {
        private readonly IRoleService _roleService;

        public Migrations(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public int Create()
        {
            _roleService.CreateRole("ivMember");
            _roleService.CreatePermissionForRole("ivMember", "ivUseMyClub");

            return 1;
        }
    }
}
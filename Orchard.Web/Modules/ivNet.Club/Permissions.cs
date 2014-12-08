
using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace ivNet.Club
{
    public class Permissions : IPermissionProvider
    {

        public static readonly Permission ivUseMyClub = new Permission
        {
            Description = "Access my-club website page",
            Name = "ivUseMyClub"
        };

        public static readonly Permission ivMyRegistration = new Permission
        {
            Description = "Access my registration details",
            Name = "ivMyRegistration"
        };

        public static readonly Permission ivConfiguration = new Permission
        {
            Description = "Manage club configuration",
            Name = "ivConfiguration"
        };

        public static readonly Permission ivManageMembers = new Permission
        {
            Description = "Manage club members",
            Name = "ivManageMembers"
        };

        public static readonly Permission ivManageFixtures = new Permission
        {
            Description = "Manage club fixtures",
            Name = "ivManageFixtures"
        };

        public Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ivUseMyClub,
                ivConfiguration,
                ivManageMembers,
                ivMyRegistration,
                ivManageFixtures
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return null;
        }
    }
}
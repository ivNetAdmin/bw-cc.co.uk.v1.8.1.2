
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

        //public static readonly Permission ivManageRules = new Permission
        //{
        //    Description = "Manage club rules",
        //    Name = "ivManageRules"
        //};

        //public static readonly Permission ivManageMembers = new Permission
        //{
        //    Description = "Manage club members",
        //    Name = "ivManageMembers"
        //};


        public Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ivUseMyClub,
               // ivManageRules,
               // ivManageMembers,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return null;
        }
    }
}
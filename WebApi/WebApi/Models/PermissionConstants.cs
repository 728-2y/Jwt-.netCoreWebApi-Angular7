using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public static class PermissionConstants
    {
        public const string PERMISSION_CLAIM_TYPE = "https://localhost:44332/api/claims/permission";

        public struct SystemPermissions
        {
            public const string MANAGE_FUNCTION = "system.function.manage";
            public const string MANAGE_LIST = "system.list";
            public const string MANAGE_ADD = "system.add";
            public const string MANAGE_UPDATE = "system.update";
            public const string MANAGE_DELETE = "system.delete";
        }

        public static List<string> SystemPermissionsSet
        {
            get
            {
                return new List<string>{
                    SystemPermissions.MANAGE_FUNCTION,
                    SystemPermissions.MANAGE_LIST,
                    SystemPermissions.MANAGE_ADD,
                    SystemPermissions.MANAGE_UPDATE,
                    SystemPermissions.MANAGE_DELETE
                };
            }
        }
    }
}

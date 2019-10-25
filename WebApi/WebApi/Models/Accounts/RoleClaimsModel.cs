using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Accounts
{
    public class RoleClaimsModel
    {
        [Required]
        public string RoleName { get; set; }

        [Required]
        public string[] Permissions { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Accounts
{
    public class UserRoleModel
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}

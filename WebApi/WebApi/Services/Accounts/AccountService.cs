using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.ApplicationDbContext;
using WebApi.Services.Accounts.Exceptions;

namespace WebApi.Services.Accounts
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly RoleManager<IdentityRole> _roleManger;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManger = userManager;
            _roleManger = roleManager;
        }

        public async Task<string> AddRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Invalid role name");
            }

            var existRole = await _roleManger.FindByNameAsync(roleName);
            if (existRole != null)
            {
                throw new DuplicateRoleException();
            }

            var role = new IdentityRole(roleName);
            await _roleManger.CreateAsync(role);

            return role.Id;
        }

        public async Task<IdentityResult> CreateAccountAsync(string email, string password, string roleName)
        {
            var user = await _userManger.FindByNameAsync(email);
            if(user !=null)
            {
                throw new AccountExistedException();
            }

            IdentityResult result = null;
            var account = new ApplicationUser()
            {
                Email = email,
                UserName = email
            };
            result = await _userManger.CreateAsync(account, password);
            
            if(result.Succeeded)
            {
               result= await _userManger.AddToRoleAsync(account, roleName);
            }

            return result;
        }
    }
}

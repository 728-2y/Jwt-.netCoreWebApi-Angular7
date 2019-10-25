using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Data.ApplicationDbContext;
using WebApi.Models;
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

        public async Task AddRoleClaimsAsync(string roleName,string[] permissions)
        {
            if (string.IsNullOrEmpty(roleName) || permissions.Length<1)
            {
                throw new ArgumentNullException("role or permissions is null");
            }

            var existRole = await _roleManger.FindByNameAsync(roleName);
            if (existRole == null)
            {
                existRole = new IdentityRole(roleName);
                await _roleManger.CreateAsync(existRole);
            }

            var existClaims = await _roleManger.GetClaimsAsync(existRole);

            var addClaims = permissions.Where(it => existClaims.Select(cl => cl.Value).Contains(it) == false);

            foreach (var claimValue in addClaims)
            {
                var claim = new Claim(PermissionConstants.PERMISSION_CLAIM_TYPE,claimValue);
                await _roleManger.AddClaimAsync(existRole,claim);
            }
        }

        public async Task<IdentityResult> RemoveRoleAsync(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var existRole = await _roleManger.FindByNameAsync(roleName);
            if (existRole == null)
            {
                throw new RoleNotFoundException();
            }

            var result = await _roleManger.DeleteAsync(existRole);

            return result;
        }

        public async Task RemoveClaimsAsync(string roleName, string[] permissions)
        {
            if (string.IsNullOrEmpty(roleName) || permissions.Length < 1)
            {
                throw new ArgumentNullException("role or permissions is null");
            }

            var existRole = await _roleManger.FindByNameAsync(roleName);
            if (existRole == null)
            {
                throw new RoleNotFoundException();
            }

            var existClaims = await _roleManger.GetClaimsAsync(existRole);
            var removeClaims = existClaims.Where(it => permissions.Contains(it.Value));
            if (removeClaims == null)
            {
                throw new ClaimNotFoundException();
            }

            foreach (var claim in removeClaims)
            {
                await _roleManger.RemoveClaimAsync(existRole, claim);
            }
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
            
            if(result.Succeeded && !string.IsNullOrEmpty(roleName))
            {
                result = await _userManger.AddToRoleAsync(account, roleName);
            }

            return result;
        }
    }
}

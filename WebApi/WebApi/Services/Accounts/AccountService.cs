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

        public async Task<string> UpdateRoleClaimsAsync(string roleName,string[] permissions)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("role is null");
            }

            var existRole = await _roleManger.FindByNameAsync(roleName);
            if (existRole == null)
            {
                existRole = new IdentityRole(roleName);
                await _roleManger.CreateAsync(existRole);
            }

            var existClaims = await _roleManger.GetClaimsAsync(existRole);

            // add claims
            var addClaims = permissions.Where(it => existClaims.Select(cl => cl.Value).Contains(it) == false);
            if (addClaims.Count() > 0)
            {
                foreach (var claimValue in addClaims)
                {
                    var claim = new Claim(PermissionConstants.PERMISSION_CLAIM_TYPE, claimValue);
                    await _roleManger.AddClaimAsync(existRole, claim);
                }
            }

            // remove claims
            var removeClaims = existClaims.Where(it => permissions.Contains(it.Value) == false);
            if (removeClaims.Count() > 0)
            {
                foreach (var claim in removeClaims)
                {
                    await _roleManger.RemoveClaimAsync(existRole, claim);
                }
            }

            return existRole.Id;
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
                // if do not have role, add new role
                if (!await _roleManger.RoleExistsAsync(roleName))
                {
                    await AddRoleAsync(roleName);
                }
                result = await _userManger.AddToRoleAsync(account, roleName);
            }

            return result;
        }

        public async Task<IdentityResult> UpdateUserRoleAsync(string email, string roleName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("email or role name is null, please confirmed again.");
            }

            IdentityResult result = null;

            var user = await _userManger.FindByNameAsync(email);
            if (user != null)
            {
                var currentUserRoles = await _userManger.GetRolesAsync(user);
                if (!currentUserRoles.Any(it => it == roleName))
                {
                    if (await _roleManger.RoleExistsAsync(roleName))
                    {
                        result = await _userManger.AddToRoleAsync(user, roleName);
                    }
                    else
                    {
                        throw new RoleNotFoundException();
                    }
                }
                else
                {
                    throw new AccountRoleExistedException();
                }
            }
            else
            {
                throw new AccountNotFoundException();
            }

            return result;
        }
    }
}

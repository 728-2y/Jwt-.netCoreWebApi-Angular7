using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Roles;
using WebApi.Services.Accounts;
using WebApi.Services.Accounts.Exceptions;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(AccountService accountService,RoleManager<IdentityRole> roleManager )
        {
            _accountService = accountService;
            _roleManager = roleManager;
        }
        // GET: api/Role
        [HttpGet]
        public IActionResult Get()
        {
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        // GET: api/Role/5
        [HttpGet("{id}", Name = "RoleGet")]
        public async Task<IActionResult> Get(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var result = await _roleManager.GetClaimsAsync(role);
            return Ok(result);
        }

        // POST: api/Role
        [HttpPost("{roleName}")]
        public async Task<IActionResult> Post(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("role name is not null");
            }

            try
            {
                var roleId = await _accountService.AddRoleAsync(roleName);
                return Content(roleId);
            }
            catch (ArgumentNullException ax)
            {
                return BadRequest(ax.Message);
            }
            catch (DuplicateRoleException dx)
            {
                return BadRequest("Role exists already,please try another.");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Role/5
        [HttpPut("{roleName}")]
        public async Task<IActionResult> UpdateRoleClaims(string roleName, [FromBody] RoleClaimsModel roleClaims)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("the role name is not null, please add role name");
            }

            try
            {
                var id = await _accountService.UpdateRoleClaimsAsync(roleName, roleClaims.Permissions);

                return Content(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("role id is not null, please add role id");
            }

            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                var result = await _accountService.RemoveRoleAsync(role.Name);
                if (result.Succeeded)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest($"delete role {id} failed");
                }
            }
            catch (ArgumentNullException anx)
            {
                return BadRequest(anx.Message);
            }
            catch (RoleNotFoundException rx)
            {
                return BadRequest("not found the role,please confirmed again");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

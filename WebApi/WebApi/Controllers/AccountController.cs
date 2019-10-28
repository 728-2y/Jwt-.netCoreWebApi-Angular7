using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Data.ApplicationDbContext;
using WebApi.Models;
using WebApi.Models.Accounts;
using WebApi.Services.Accounts;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AccountService _accountService;
        private readonly IOptions<JwtTokenSetting> _jwtTokenSetting;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AccountService accountService,
            IOptions<JwtTokenSetting> jwtTokenSetting)
        {
            _signInManger = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _accountService = accountService;
            _jwtTokenSetting = jwtTokenSetting;
        }
        // GET: api/Account
        [HttpGet]
        [Authorize(Roles ="Administrator")]
        //[Authorize(Policy =PermissionConstants.SystemPermissions.MANAGE_LIST)]
        public IActionResult Get()
        {
            var result = User.Claims.Select(it => new { it.Type, it.Value });

            return Ok(result);
        }

        // GET: api/Account/5
        [HttpGet("{id}", Name = "GetAccount")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Account
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] SignUpModel signup)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("the email or password is invalid");
            }

            //await _accountService.AddRoleAsync(signup.RoleName);

            var result = await _accountService.CreateAccountAsync(signup.Email, signup.Password, signup.RoleName);
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Created(nameof(Login),result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("the email or password is invalid");
            }

            var user = await _userManager.FindByNameAsync(login.Email);
            if (user == null)
            {
                return BadRequest("the account is not register, please register");
            }

            var result = await _signInManger.PasswordSignInAsync(user, login.Password, login.IsRemeberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return BadRequest("password is error");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, roleName));
                if (_roleManager.SupportsRoleClaims)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var claims = await _roleManager.GetClaimsAsync(role);
                        foreach (var claim in claims)
                        {
                            userClaims.Add(claim);
                        }
                    }
                }
            }

            userClaims.Add(new Claim(ClaimTypes.PrimarySid,user.Id));
            userClaims.Add(new Claim(ClaimTypes.Email, user.Email));
            userClaims.Add(new Claim(ClaimTypes.Name, user.UserName));

            var secretKey = Encoding.UTF8.GetBytes( _jwtTokenSetting.Value.SecretKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);

            var issuer = _jwtTokenSetting.Value.Issuer;
            var securityToken = new JwtSecurityToken(issuer: issuer, audience: null, claims: userClaims, null, DateTime.Now.AddMinutes(30), credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(securityToken);

            return Ok(new
            {
                access_token=token
            });
        }

        // PUT: api/Account/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

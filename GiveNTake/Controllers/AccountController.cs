using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GiveNTake.Model;
using GiveNTake.Model.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GiveNTake.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registration)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            User newUser = new User
            {
                Email = registration.Email,
                UserName = registration.Email,
                Id = registration.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, registration.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<SuccessfulLoginResult>> Login([FromBody]LoginUserDTO login)
        {
            SignInResult result = await
        _signInManager.PasswordSignInAsync(login.Email, login.Password,
        isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            User user = await _userManager.FindByEmailAsync(login.Email);
            JwtSecurityToken token = await GenerateTokenAsync(user);
            //defined
            string serializedToken = new
            JwtSecurityTokenHandler().WriteToken(token); //serialize the token
            return Ok(new SuccessfulLoginResult()
            {
                Token = serializedToken
            });
        }

        private async Task<JwtSecurityToken> GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>();

            // Loading the user Claims

            var expirationDays = _configuration.GetValue<int>
            ("JWTConfiguration:TokenExpirationDays");
            var siginingKey =
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>
            ("JWTConfiguration:SigningKey"));
            var token = new JwtSecurityToken
            (
                issuer: _configuration.GetValue<string>
                ("JWTConfiguration:Issuer"),
                audience: _configuration.GetValue<string>
                ("JWTConfiguration:Audience"),
                claims: claims,
                expires:
                DateTime.UtcNow.Add(TimeSpan.FromDays(expirationDays)),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new
                SymmetricSecurityKey(siginingKey),
                    SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}
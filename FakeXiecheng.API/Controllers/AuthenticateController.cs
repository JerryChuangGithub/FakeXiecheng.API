using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FakeXiecheng.API.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FakeXiecheng.API.Controllers
{
    [ApiController]
    [Route("api/authenticate")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly UserManager<IdentityUser> _userManager;

        public AuthenticateController(
            IConfiguration configuration,
            UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        
        [HttpGet("login")]
        public IActionResult Login([FromBody]LoginDto loginDto)
        {
            // jwt - payload
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "fake_user_id"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            // jwt - header
            const string signingAlgorithm = SecurityAlgorithms.HmacSha256;
            // signature
            var secretBytes = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            var signingKey = new SymmetricSecurityKey(secretBytes);
            var signingCredentials = new SigningCredentials(signingKey, signingAlgorithm);
            
            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials);

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenStr);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
        {
            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded == false)
            {
                return BadRequest();
            }
            
            return NoContent();
        }
    }
}
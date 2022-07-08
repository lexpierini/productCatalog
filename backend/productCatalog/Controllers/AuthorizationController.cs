using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using productCatalog.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace productCatalog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthorizationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterUser([FromBody] UserDTO userDTO)
        {
            var user = new IdentityUser
            {
                UserName = userDTO.Email,
                Email = userDTO.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, false);
            return Ok(MakeToken(userDTO));
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] UserDTO userDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(userDTO.Email, userDTO.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(MakeToken(userDTO));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login");
                return BadRequest(ModelState);
            }
        }

        private UserToken MakeToken(UserDTO userDTO)
        {
            // Define User Statements
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userDTO.Email),
                new Claim("myPet", "raksha"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Generates a key based on a symmetric algorithm
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]));

            // Generates the digital signature of the token using the Hmac algorithm and the private key
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token Expiration Time
            var expiration = _config["TokenConfiguration:ExpireHours"];
            var expirationParse = DateTime.UtcNow.AddHours(double.Parse(expiration));

            // Class representing a JWT token and generating the token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["TokenConfiguration:Issuer"],
                audience: _config["TokenConfiguration:Audience"],
                claims: claims,
                expires: expirationParse,
                signingCredentials: credentials);

            // Returns the data with the token and information
            return new UserToken()
            {
                Authenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expirationParse,
                Message = "Token Jwt Ok"
            };
        }
    }
}

//{
//  "email": "admin@admin.com",
//  "password": "Admin@123",
//  "confirmPassword": "admin"
//}
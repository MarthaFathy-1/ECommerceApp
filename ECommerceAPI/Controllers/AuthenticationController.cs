using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private UserManager<AppUser> userManager;

        public AuthenticationController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            AppUser appUser = new AppUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                HomeAddress = registerDTO.HomeAddress,
                PhoneNumber = registerDTO.PhoneNumber
            };

            var existingEmail = await userManager.FindByEmailAsync(registerDTO.Email);
            var existingUserName = await userManager.FindByNameAsync(registerDTO.UserName);
            if (existingEmail != null)
                return BadRequest("Email already exists");
            if (existingUserName != null)
                return BadRequest("Username already exists");

            var result = await userManager.CreateAsync(appUser, registerDTO.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(appUser, "Viewer");
                return Ok("User registered successfully");
            }
            else
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO userFromRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //check if the user exists
            AppUser userFromDb = await userManager.FindByNameAsync(userFromRequest.UserName);
            if (userFromDb == null) return BadRequest("Invalid username or password");

            var isPasswordValid = await userManager.CheckPasswordAsync(userFromDb, userFromRequest.Password);
            if (!isPasswordValid) return BadRequest("Invalid username or password");

            var userRoles = await userManager.GetRolesAsync(userFromDb);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userFromDb.Id),
                new Claim(ClaimTypes.Name, userFromDb.UserName),
                new Claim(ClaimTypes.Email, userFromDb.Email),
                new Claim(JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Asdj2333DCTTyydd45sdjhdsfdsjhsdfGFSSSS554wwew"));
            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Generate the token
            JwtSecurityToken myToken = new JwtSecurityToken(
                issuer: "http://localhost:46926/",
                audience: "http://localhost:4200/",
                expires: DateTime.UtcNow.AddHours(2),
                claims: claims,
                signingCredentials: signingCredentials
            );

            //generate the token response
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(myToken),
                expiration = myToken.ValidTo,
                roles = userRoles
            });

        }
    }
}

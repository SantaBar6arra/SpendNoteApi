using Data;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Constants;
using WebApi.Dtos;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitofWork;

        public AuthController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitofWork = unitOfWork;
        }

        [HttpPost("signup")]
        public async Task<ActionResult> Signup([FromBody] UserDto userDto)
        {
            var u = await _unitofWork.UserRepository.GetByName(userDto.Name);
            if (u == null)
            {
                User user = new();

                PasswordHashService.CreatePasswordHash(
                    userDto.Password, out string hash, out string salt);

                user.PasswordHash = hash;
                user.Salt = salt;
                user.Name = userDto.Name;
                user.Email = userDto.Email;

                if (_unitofWork.UserRepository.Upsert(user))
                    if (await _unitofWork.Complete() > 0)
                    {
                        user = await _unitofWork.UserRepository.GetByName(user.Name);
                        return Ok(user.Id);
                    }
            }
            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            User user = await _unitofWork.UserRepository.GetByName(userDto.Name);
            if (user != null)
            {
                if (PasswordHashService.VerifyPasswordHash(userDto.Password, user.PasswordHash, user.Salt))
                    return new JsonResult(new TokenResult() { Token = CreateToken(user) });
                
                return BadRequest("passwords do not match");
            }
            return BadRequest("user does not exist");
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new ()
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Name, user.Name),
                new (ClaimTypes.Email, user.Email),
            };

            SymmetricSecurityKey key = new (Encoding.UTF8.GetBytes(_configuration[ConfigurationConstants.TokenSettings_TokenString]));
            SigningCredentials credentials = new (key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new ()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new ();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

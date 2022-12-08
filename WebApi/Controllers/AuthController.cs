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
using WebApi.Services.ControllerServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;

        public AuthController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _service = new(unitOfWork);
        }

        [HttpPost("signup")]
        public async Task<ActionResult> Signup([FromBody] UserDto userDto)
        {
            int? userId = await _service.Signup(userDto);
            if (userId != null)
                return Ok(userId);
            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            TokenResult? token = await _service.Login(userDto);
            if (token != null)
                return Ok(token);
            return BadRequest();
        }
    }
}

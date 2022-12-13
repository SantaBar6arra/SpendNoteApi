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
    public class AuthController : BaseController
    {
        private readonly AuthService _service;

        public AuthController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _service = new(unitOfWork);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserDto userDto)
        {
            var serviceResponse = await _service.Signup(userDto);
            return HandleServiceResponse(serviceResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var serviceResponse = await _service.Login(userDto);
            return HandleServiceResponse(serviceResponse);
        }
    }
}

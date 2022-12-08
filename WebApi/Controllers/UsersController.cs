using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity.Validation;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Constants;
using WebApi.Dtos;
using WebApi.Services.ControllerServices;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UserService _userService;

        public UsersController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            int userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            _userService = new (unitOfWork, userId);
        }

        [HttpPost("request-password-update")]
        public async Task<IActionResult> RequestPasswordUpdate([FromBody] UserDto userDto)
        {
            if(await _userService.RequestPasswordUpdate(userDto))
                return Ok();
            return BadRequest(HttpResponseReasons.SomethingWentWrong);
        }

        [HttpPut("submit-password-update")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            if (await _userService.UpdatePassword(updatePasswordDto))
                return Ok();
            return BadRequest(HttpResponseReasons.SomethingWentWrong);
        }
    }
}
using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity.Validation;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Constants;
using WebApi.Dtos;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmailSenderService _emailSenderService;

        private int _userId;

        public UsersController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            _emailSenderService = new ();
        }

        [HttpPost("request-password-update")]
        public async Task<IActionResult> RequestPasswordUpdate([FromBody] UserDto userDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (PasswordHashService.VerifyPasswordHash(userDto.Password, user.PasswordHash, user.Salt) && user.Email == userDto.Email)
            {
                var verificationCode = await _unitOfWork.VerificationCodeRepository.GetByUserId(_userId);

                if (verificationCode != null)
                    if (_unitOfWork.VerificationCodeRepository.Remove(verificationCode))
                        if (await _unitOfWork.Complete() <= 0)
                            return Problem();
                            
                string guid = Guid.NewGuid().ToString();
                verificationCode = new()
                {
                    User = user,
                    Code = guid
                };

                if (_unitOfWork.VerificationCodeRepository.Upsert(verificationCode))
                    if (await _unitOfWork.Complete() <= 0)
                        return Problem();

                _emailSenderService.SendEmail(userDto.Email, guid);                

                return Ok();
            }

            return BadRequest(HttpResponseReasons.SomethingWentWrong);
        }

        [HttpPut("submit-password-update")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            var verificationCode = 
                await _unitOfWork.VerificationCodeRepository.GetByGuid(updatePasswordDto.Guid);

            if (verificationCode == null)
                return NotFound("not found");

            if(verificationCode.User.Id == _userId)
            {
                var user = await _unitOfWork.UserRepository.GetById(verificationCode.User.Id);

                if (user == null)
                    return BadRequest();

                PasswordHashService.CreatePasswordHash(
                    updatePasswordDto.Password, out string hash, out string salt);

                user.PasswordHash = hash;
                user.Salt = salt;

                if (_unitOfWork.UserRepository.Upsert(user) 
                    && _unitOfWork.VerificationCodeRepository.Remove(verificationCode))
                    if (await _unitOfWork.Complete() > 0)
                        return Ok("pwd updated");
            }

            return BadRequest(HttpResponseReasons.SomethingWentWrong);
        }
    }
}
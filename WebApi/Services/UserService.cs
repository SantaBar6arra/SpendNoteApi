using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _userId;
        private readonly EmailSenderService _emailSenderService;

        public UserService(IUnitOfWork unitOfWork, int userId)
        {
            _unitOfWork = unitOfWork;
            _userId = userId;
            _emailSenderService = new();
        }

        public async Task<IActionResult> RequestPasswordUpdate(UserDto userDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (!PasswordHashService.VerifyPasswordHash(userDto.Password, user.PasswordHash, user.Salt) || user.Email != userDto.Email)
                return new ForbidResult();

            var verificationCode = await _unitOfWork.VerificationCodeRepository.GetByUserId(_userId);

            if (verificationCode != null)
                if (_unitOfWork.VerificationCodeRepository.Remove(verificationCode))
                    if (await _unitOfWork.Complete() <= 0)
                        return new ObjectResult(null) { StatusCode = (int)HttpStatusCode.InternalServerError };

            verificationCode = CreateVerificationCode(user);

            if (_unitOfWork.VerificationCodeRepository.Upsert(verificationCode))
                if (await _unitOfWork.Complete() <= 0)
                    return new ObjectResult(null) { StatusCode = (int)HttpStatusCode.InternalServerError };

            _emailSenderService.SendEmail(userDto.Email, verificationCode.Code);

            return new OkResult();
        }

        public async Task<IActionResult> UpdatePassword(UpdatePasswordDto updatePasswordDto)
        {
            var verificationCode =
                await _unitOfWork.VerificationCodeRepository.GetByGuid(updatePasswordDto.Guid);

            if (verificationCode == null)
                return new NotFoundResult();

            if (verificationCode.User.Id != _userId)
                return new ForbidResult();

            var user = await _unitOfWork.UserRepository.GetById(verificationCode.User.Id);

            if (user == null)
                return new NotFoundResult();

            PasswordHashService.CreatePasswordHash(
                updatePasswordDto.Password, out string hash, out string salt);

            user.PasswordHash = hash;
            user.Salt = salt;

            if (_unitOfWork.UserRepository.Upsert(user)
                && _unitOfWork.VerificationCodeRepository.Remove(verificationCode))
                if (await _unitOfWork.Complete() > 0)
                    return new OkResult();

            return new ObjectResult(null) { StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        private VerificationCode CreateVerificationCode(User user)
        {
            string guid = Guid.NewGuid().ToString();
            return new()
            {
                User = user,
                Code = guid
            };
        }
    }
}

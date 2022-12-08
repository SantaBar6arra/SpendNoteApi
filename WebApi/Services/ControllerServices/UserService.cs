using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Services.ControllerServices
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

        public async Task<bool> RequestPasswordUpdate(UserDto userDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (!PasswordHashService.VerifyPasswordHash(userDto.Password, user.PasswordHash, user.Salt) || user.Email != userDto.Email)
                return false;

            var verificationCode = await _unitOfWork.VerificationCodeRepository.GetByUserId(_userId);

            if (verificationCode != null)
                if (_unitOfWork.VerificationCodeRepository.Remove(verificationCode))
                    if (await _unitOfWork.Complete() <= 0)
                        return false;

            verificationCode = CreateVerificationCode(user);

            if (_unitOfWork.VerificationCodeRepository.Upsert(verificationCode))
                if (await _unitOfWork.Complete() <= 0)
                    return false;

            _emailSenderService.SendEmail(userDto.Email, verificationCode.Code);

            return true;
        }

        public async Task<bool> UpdatePassword(UpdatePasswordDto updatePasswordDto)
        {
            var verificationCode =
                await _unitOfWork.VerificationCodeRepository.GetByGuid(updatePasswordDto.Guid);

            if (verificationCode == null)
                return false;

            if (verificationCode.User.Id != _userId)
                return false;

            var user = await _unitOfWork.UserRepository.GetById(verificationCode.User.Id);

            if (user == null)
                return false;

            PasswordHashService.CreatePasswordHash(
                updatePasswordDto.Password, out string hash, out string salt);

            user.PasswordHash = hash;
            user.Salt = salt;

            if (_unitOfWork.UserRepository.Upsert(user)
                && _unitOfWork.VerificationCodeRepository.Remove(verificationCode))
                if (await _unitOfWork.Complete() > 0)
                    return true;

            return false;
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

﻿using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Services.ControllerServices
{
    public class AuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _configuration = new ConfigurationBuilder().AddJsonFile(ConfigurationConstants.ConfigurationFileName).Build();
        }

        public async Task<int?> Signup(UserDto userDto)
        {
            var dbUser = await _unitOfWork.UserRepository.GetByName(userDto.Name);

            if (dbUser != null)
                return null;

            PasswordHashService.CreatePasswordHash(
                userDto.Password, out string hash, out string salt);

            User user = new()
            {
                PasswordHash = hash,
                Salt = salt,
                Name = userDto.Name,
                Email = userDto.Email,
            };

            if (_unitOfWork.UserRepository.Upsert(user))
                if (await _unitOfWork.Complete() <= 0)
                    return null;
            
            user = await _unitOfWork.UserRepository.GetByName(user.Name);
            return user.Id;           
        }

        public async Task<TokenResult?> Login(UserDto userDto)
        {
            User user = await _unitOfWork.UserRepository.GetByName(userDto.Name);

            if (user == null)
                return null;

            if (PasswordHashService.VerifyPasswordHash(userDto.Password, user.PasswordHash, user.Salt))
                return new() { Token = CreateToken(user) };

            return null;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Name, user.Name),
                new (ClaimTypes.Email, user.Email),
            };

            SymmetricSecurityKey key = new(
                Encoding.UTF8.GetBytes(_configuration[ConfigurationConstants.TokenSettings_TokenString]));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

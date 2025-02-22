using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Dtos;
using Backend.Domain.Entities.Account;

namespace Backend.Application.Interfaces.Account
{
    public interface ITokenService
    {
        Task<string> CreateToken(User appUser, string role = null, bool isAccessToken = true);
        Task<UserInfoByTokenDto> UserInfoByTokenAsync(string accessToken);
        Task<bool> TokenExpired(string token);
    }
}
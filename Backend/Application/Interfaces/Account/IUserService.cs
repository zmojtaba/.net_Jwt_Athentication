using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Dtos;
using Backend.Presentation.API.Dtos.Account;

namespace Backend.Application.Interfaces.Account
{
    public interface IUserService
    {
        public Task<Response> Register(RegisterDto registerDto);
        public Task<Response> RefreshToken(string refreshToken, string accessToken);

        public Task<Response> LogIn(LogInDto logInDto);

        public Task<Response> ResetPass(ResetPassDto resetPassDto, string accessToken);
    }
}
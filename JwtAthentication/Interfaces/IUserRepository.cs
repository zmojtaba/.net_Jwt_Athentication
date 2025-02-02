using Microsoft.AspNetCore.Identity;
using SecondApp.Dtos.Account;
using SecondApp.Models;

namespace SecondApp.Interfaces
{
    public interface IUserRepository
    {
        Task<RegisterRepoRes> Register(RegisterDto registerDto);
        Task<string> RefreshToken(string accessToken, string refreshToken);

        Task<RepositoryResponse> Login(LogInDto loginModel);

        Task<RepositoryResponse> ResetPassword(ResetPasswordDto resetDto, string accessToken);
    }
}

using SecondApp.Dtos.Account;
using SecondApp.Models;

namespace SecondApp.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser appUser, string role=null, bool isAccessToken=true);
        Task<UserInfoByTokenDto> UserInfoByToken(string accessToken);
        Task<bool> TokenExpired(string token);
    }
}

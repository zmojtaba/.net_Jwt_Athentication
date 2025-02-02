using Microsoft.AspNetCore.Identity;
using SecondApp.Models;

namespace SecondApp.Dtos.Account
{
    public class RegisterResponseDto
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public AppUser User { get; set; } = new AppUser();
    }
}

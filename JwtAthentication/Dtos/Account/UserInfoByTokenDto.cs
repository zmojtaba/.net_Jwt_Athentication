using SecondApp.Models;

namespace SecondApp.Dtos.Account
{
    public class UserInfoByTokenDto
    {
        public AppUser AppUser { get; set; }= new AppUser();
        public string Role { get; set; } = string.Empty;    
    }
}

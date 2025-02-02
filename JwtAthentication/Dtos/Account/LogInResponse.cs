using SecondApp.Models;

namespace SecondApp.Dtos.Account
{
    public class LogInResponse
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public AppUser AppUser { get; set; }

    }
}

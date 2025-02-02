using Microsoft.AspNetCore.Identity;
using SecondApp.Models;

namespace SecondApp.Dtos.Account
{
    public class RegisterRepoRes
    {
        public AppUser? AppUser { get; set; }
        public string Status { get; set; } = string.Empty;
        public IdentityResult? UserModel { get; set; }
        public object Message { get; set; } = new object();
        public int StatusCode { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace SecondApp.Dtos.Account
{
    public class ResetPasswordDto
    {
        [Required]
        //[Authorize]
        public string UserName { get; set; }
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}

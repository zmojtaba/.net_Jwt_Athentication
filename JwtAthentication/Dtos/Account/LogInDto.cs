using System.ComponentModel.DataAnnotations;

namespace SecondApp.Dtos.Account
{
    public class LogInDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

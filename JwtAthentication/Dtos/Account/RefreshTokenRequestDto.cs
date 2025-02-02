using System.ComponentModel.DataAnnotations;

namespace SecondApp.Dtos.Account
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Presentation.API.Dtos.Account
{
    public class ResetPassDto
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
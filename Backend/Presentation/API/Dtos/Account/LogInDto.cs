using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Presentation.API.Dtos.Account
{
    public class LogInDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
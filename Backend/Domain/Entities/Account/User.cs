using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Backend.Domain.Entities.Account
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Entities.Account;

namespace Backend.Application.Dtos
{
    public class RegisterResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public User User { get; set; }
    }
}
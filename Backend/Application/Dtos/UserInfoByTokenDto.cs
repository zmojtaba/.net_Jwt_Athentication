using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Entities.Account;

namespace Backend.Application.Dtos
{
    public class UserInfoByTokenDto
    {
        public User User { get; set; }
        public string Role { get; set; }
    }
}
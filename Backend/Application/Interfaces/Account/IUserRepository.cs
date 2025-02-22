using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Entities.Account;
using Backend.Presentation.API.Dtos.Account;
using Microsoft.AspNetCore.Identity;

namespace Backend.Application.Interfaces.Account
{
    public interface IUserRepository
    {
        public Task<IdentityResult> CreateAsync(User user, string password);
        public Task<User> FindUserByUserNameAsync(string userName);

        public Task<bool> CheckRole(string role);

        public Task<IdentityResult> AddRoleToUserAsync(User user, string role);
        public Task<bool> UpdateUserRefreshTokenAsync(User user, string refreshToken);
        public Task<IdentityResult> DeleteUserAsync(User user);
        public Task<bool> CheckPasswordAsync(User user, string password);
        public Task<IList<string>> GetUserRoleAsync(User user);

        public Task<IdentityResult> ChangeUserPasswordAsync(User user, string OldPassword, string newPassword);

        public Task<IdentityResult> UpdateUserAsync(User user);
    }
}
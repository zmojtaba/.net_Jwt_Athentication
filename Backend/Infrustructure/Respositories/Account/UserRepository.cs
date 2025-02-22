using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Interfaces.Account;
using Backend.Domain.Entities.Account;
using Backend.Presentation.API.Dtos.Account;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrustructure.Respositories.Account
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<User> FindUserByUserNameAsync(string userName)
        {
            User? user = await _userManager.FindByNameAsync(userName);
            if (user == null) return null;
            return user;
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            IdentityResult result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<bool> CheckRole(string role)
        {
            bool result = await _roleManager.RoleExistsAsync(role);
            if (!result)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                return true;
            }
            return true;
        }

        public async Task<IdentityResult> AddRoleToUserAsync(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> UpdateUserRefreshTokenAsync(User user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IList<string>> GetUserRoleAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> ChangeUserPasswordAsync(User user, string OldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, OldPassword, newPassword);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}
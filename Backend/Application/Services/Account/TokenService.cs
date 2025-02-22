using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Backend.Application.Dtos;
using Backend.Application.Interfaces.Account;
using Backend.Domain.Entities.Account;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Application.Services.Account
{
    public class TokenService : ITokenService
    {
        public JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepo;
        public TokenService(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
            _userRepo = userRepository;
        }

        public async Task<string> CreateToken(User appUser, string role = null, bool isAccessToken = true)
        {
            if (isAccessToken && !role.IsNullOrEmpty())
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, appUser.UserName),
                    new Claim(JwtRegisteredClaimNames.Name, appUser.UserName),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("token_type", "access_token")
                };
                var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddHours(12),
                    SigningCredentials = creds,
                    Issuer = _config["JWT:Issuer"],
                    Audience = _config["JWT:Audience"]

                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            else //when it refresh token
            {

                var claims = new List<Claim>
                {
                    new Claim("token_type", "refresh_token")
                };
                var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(7),
                    SigningCredentials = creds,
                    Issuer = _config["JWT:Issuer"],
                    Audience = _config["JWT:Audience"]

                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                appUser.RefreshToken = tokenHandler.WriteToken(token);
                // await _userManager.UpdateAsync(appUser);

                return tokenHandler.WriteToken(token);
            }

        }

        public async Task<UserInfoByTokenDto> UserInfoByTokenAsync(string accessToken)
        {
            var jwtToken = tokenHandler.ReadJwtToken(accessToken);
            string? userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            string? userRole = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (userRole == null || userName == null) return null;

            User? appUser = await _userRepo.FindUserByUserNameAsync(userName.ToUpperInvariant());

            if (appUser == null) return null;

            return new UserInfoByTokenDto
            {
                User = appUser,
                Role = userRole,
            };
        }

        public async Task<bool> TokenExpired(string token)
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            long? tokenExp = long.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value);

            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            bool canRefresh = true ? (tokenExp >= currentTime) : false;
            return canRefresh;
        }
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SecondApp.Dtos.Account;
using SecondApp.Interfaces;
using SecondApp.Models;

namespace SecondApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;
        public JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        public TokenService(IConfiguration config, UserManager<AppUser> userManager) { 
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
            _userManager = userManager;
        }
        public async Task<string> CreateToken(AppUser appUser, string role=null, bool isAccessToken = true)
        {

            //if (string.Equals(tokenType, "access_token", StringComparison.OrdinalIgnoreCase))
            if (isAccessToken && !role.IsNullOrEmpty())
                {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, appUser.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
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
                Console.WriteLine($"^^^^^^^^^^^^^^^^^^^^^^^^^^222 {role},   {isAccessToken}");

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
                await _userManager.UpdateAsync(appUser);

                return tokenHandler.WriteToken(token);
            }

                    
        }


        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"])),
                ValidateLifetime = true // Ensures the token is not expired
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is JwtSecurityToken jwtSecurityToken)
                {
                    // Verify signing algorithm
                    if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                        throw new SecurityTokenException("Invalid signing algorithm");

                    // Verify expiration
                    if (jwtSecurityToken.ValidTo < DateTime.UtcNow)
                        throw new SecurityTokenExpiredException("Token has expired");

                    return principal;
                }

                throw new SecurityTokenException("Invalid token format");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> TokenExpired(string token)
        {

            //var principal = GetPrincipalFromExpiredToken(token);

            //Console.WriteLine($"*******1111####******{principal}");

            var jwtToken = tokenHandler.ReadJwtToken(token);
            long? tokenExp = long.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value);

            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            bool canRefresh = true ? (tokenExp >= currentTime) : false;
            return canRefresh;
        }

        public async Task<UserInfoByTokenDto> UserInfoByToken(string accessToken)
        {
            var jwtToken = tokenHandler.ReadJwtToken(accessToken);
            string? userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            string? userRole = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            AppUser? appUser = await _userManager.FindByNameAsync(userName.ToUpperInvariant());
            if (appUser == null || userRole == null || userName == null) return null;
            return new UserInfoByTokenDto
            {
                AppUser = appUser,
                Role = userRole,
            };

        }
    }
}

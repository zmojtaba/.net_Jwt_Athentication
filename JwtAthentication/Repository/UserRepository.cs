using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SecondApp.Dtos.Account;
using SecondApp.Interfaces;
using SecondApp.Models;

namespace SecondApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        public List<string> _userRoles = new List<string>()
        {
            "admin", "user"
        };
        public UserRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService) { 
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<RepositoryResponse> Login(LogInDto loginModel)
        {
            AppUser? user = await _userManager.FindByNameAsync(loginModel.UserName);
            AppUser? userOne = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginModel.UserName);
            if (user == null) { return new RepositoryResponse
            {
                StatuCode = 404,
                Message = "UserName not found!"
            }; }

            if (await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                IList<string> userRole =  await _userManager.GetRolesAsync(user);
                if (userRole.Count > 0) { 
                    string accessToken = await _tokenService.CreateToken(user, userRole[0]);
                    return new RepositoryResponse
                    {
                        StatuCode = 200,
                        Message = new LogInResponse()
                        {

                            AccessToken = accessToken,
                            RefreshToken = await _tokenService.CreateToken(user),
                            AppUser = user,
                        }
                    };
                        
                        
;
                }
                else { return new RepositoryResponse
                {
                    StatuCode = 400,
                    Message = "User has not Role!"
                }; }

            }
            else {
                return new RepositoryResponse
                {
                    StatuCode = 400,
                    Message = "Password is not correct!"
                };
            }

        }

        public async Task<string> RefreshToken(string accessToken, string refreshToken)
        {
            try
            {
                // find user by token


                UserInfoByTokenDto userInfo = await _tokenService.UserInfoByToken(accessToken);
                if (userInfo == null) {
                    return "Token is not Valid!";
                }

                if (string.Equals(userInfo.AppUser.RefreshToken, refreshToken, StringComparison.OrdinalIgnoreCase))
                {
                    // condition for checking expiratoin of refresh token:


                    if (await _tokenService.TokenExpired(refreshToken))
                    {
                        string NewAccessToken = await _tokenService.CreateToken(userInfo.AppUser, userInfo.Role);
                        return NewAccessToken;

                    }
                    else
                    {
                        return "Refresh token Expired. please login again!";
                    }
                }
                else
                {
                    return "Refresh Token is not valid";
                }


            }
            catch (Exception e)
            {
                return e.ToString();
            }


        }

        public async Task<RegisterRepoRes> Register(RegisterDto registerDto)
        {


            try
            {

                registerDto.Role = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(registerDto.Role);

                Console.WriteLine($":::::::::::::::::{registerDto.Role}");
                string normalizedUserName = registerDto.UserName.ToUpperInvariant();
                Console.WriteLine($":::::::::::::::::{normalizedUserName}");

                AppUser? user = await _userManager.FindByNameAsync(normalizedUserName);


                Console.WriteLine($"------------ {user}");
                if (user == null)
                {
                    AppUser appUser = new AppUser
                    {
                        Email = registerDto.Email,
                        UserName = registerDto.UserName
                    };
                    IdentityResult userModel = await _userManager.CreateAsync(appUser, registerDto.Password);
                    if (userModel.Succeeded)
                    {
                        if (_userRoles.Contains(registerDto.Role, StringComparer.OrdinalIgnoreCase))
                        {

                            bool role = await _roleManager.RoleExistsAsync(registerDto.Role);
                            if (!role)
                            {
                                await _roleManager.CreateAsync(new IdentityRole(registerDto.Role));

                            }

                            IdentityResult roleResult =  await _userManager.AddToRoleAsync(appUser, registerDto.Role);
                            if (roleResult.Succeeded)
                            {
                                return new RegisterRepoRes
                                {
                                    AppUser = appUser,
                                    UserModel = userModel,
                                    Status = "Success",
                                    Message = "User registre successfully!",
                                    StatusCode = 200,
                                };
                            }
                            else
                            {
                                string errorMessages = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                                return new RegisterRepoRes
                                {
                                    Status = "Error",
                                    Message = roleResult.Errors,
                                    StatusCode = 500
                                };
                                
                            }


                        }
                        else {
                            return new RegisterRepoRes
                            {
                                Status = "Error",
                                Message = $"{registerDto.Role} is not valid Role. Please choose Either Admin or User Role",
                                StatusCode = 400
                            }; 
                        }

                    }
                    else
                    {
                        string errorMessage = string.Join("; ", userModel.Errors.Select(e => e.Description));
                        return new RegisterRepoRes
                        {
                            Status = "Error",
                            Message = userModel.Errors,
                            StatusCode = 500
                        };
                    }


                }
                else if (user != null) {
                    return new RegisterRepoRes{
                        Status = "Error",
                        Message = $"{user.UserName} user already exists!",
                        StatusCode = 409
                    };
                }
                else
                {
                    return new RegisterRepoRes
                    {
                        Status = "Error",
                        Message = "----------------------- in else of user is null",
                        StatusCode = 409
                    };
                }
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return new RegisterRepoRes
                    {
                        Status = "Error",
                        Message = e,
                        StatusCode = 500
                    };
                }
                else
                {
                    return new RegisterRepoRes
                    {
                        Status = "Error",
                        Message = "An unexpected error occurred. Please try again later.",
                        StatusCode = 500
                    };
                }
            }


        }

        public async Task<RepositoryResponse> ResetPassword(ResetPasswordDto resetDto, string accessToken)
        {
            AppUser? user = await _userManager.FindByNameAsync(resetDto.UserName);

            if (user == null)
            {
                return new RepositoryResponse
                {
                    StatuCode = 401,
                    Message = "UserName does not exist!"
                };
                  
            }
            UserInfoByTokenDto userInfo = await _tokenService.UserInfoByToken(accessToken);

            if (userInfo == null)
            {
                return new RepositoryResponse
                {
                    StatuCode = 400,
                    Message = "Token is not valid!"
                };
            }

            bool isMatchingUser = userInfo.AppUser.UserName.Equals(resetDto.UserName, StringComparison.OrdinalIgnoreCase);

            if (!isMatchingUser)
            {
                return new RepositoryResponse
                {
                    StatuCode = 400,
                    Message = "Token is not correct for this username!"
                };
            }



            IdentityResult result = await _userManager.ChangePasswordAsync(user, resetDto.OldPassword, resetDto.NewPassword);
            await _userManager.UpdateAsync(user);
            if (result.Succeeded) return  new RepositoryResponse
            {
                StatuCode = 200, 
                Message = "Password has been changed!"
            };
            return new RepositoryResponse
            {
                StatuCode = 400,
                Message = result.Errors
            };

        }


        //public async Task<>
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Dtos;
using Backend.Application.Interfaces.Account;
using Backend.Domain.Entities.Account;
using Backend.Presentation.API.Dtos.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Backend.Application.Services.Account
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        public List<string> _userRoles = new List<string>()
        {
            "admin", "user"
        };
        public UserService(IUserRepository userRepository, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _userRepo = userRepository;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<Response> Register(RegisterDto registerDto)
        {
            try
            {
                registerDto.Role = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(registerDto.Role);

                User? user = await _userRepo.FindUserByUserNameAsync(registerDto.UserName);
                if (user == null)
                {
                    User newUser = new User
                    {
                        UserName = registerDto.UserName,
                    };
                    IdentityResult userModel = await _userRepo.CreateAsync(newUser, registerDto.Password);
                    if (userModel.Succeeded)
                    {
                        if (_userRoles.Contains(registerDto.Role, StringComparer.OrdinalIgnoreCase))
                        {
                            string userRole = registerDto.Role;
                            await _userRepo.CheckRole(userRole);

                            IdentityResult roleResult = await _userRepo.AddRoleToUserAsync(newUser, userRole);
                            if (roleResult.Succeeded)
                            {

                                string accessToken = await _tokenService.CreateToken(newUser, role: registerDto.Role);
                                string refreshToken = await _tokenService.CreateToken(newUser, isAccessToken: false);
                                await _userRepo.UpdateUserRefreshTokenAsync(newUser, refreshToken);

                                RegisterResponse response = new RegisterResponse
                                {
                                    AccessToken = accessToken,
                                    RefreshToken = refreshToken,
                                    User = newUser
                                };

                                return new Response
                                {
                                    StatusCode = 200,
                                    Message = response
                                };
                            }
                            else
                            {
                                await _userRepo.DeleteUserAsync(newUser);
                                return new Response
                                {
                                    StatusCode = 500,
                                    Message = roleResult.Errors
                                };
                            }

                        }
                        else
                        {
                            await _userRepo.DeleteUserAsync(newUser);
                            return new Response
                            {
                                StatusCode = 400,
                                Message = $"{registerDto.Role} is not valid Role. Please choose Either Admin or User Role",
                            };
                        }
                    }
                    else
                    {
                        await _userRepo.DeleteUserAsync(newUser);
                        return new Response
                        {
                            StatusCode = 500,
                            Message = userModel.Errors
                        };
                    }
                }
                else
                {
                    return new Response
                    {
                        Message = $"This username {user.UserName} has already exists!",
                        StatusCode = 409
                    };
                }

            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return new Response
                    {
                        Message = e,
                        StatusCode = 500
                    };
                }
                else
                {
                    return new Response
                    {
                        Message = "An unexpected error occurred. Please try again later.",
                        StatusCode = 500
                    };
                }
            }
        }


        public async Task<Response> RefreshToken(string refreshToken, string accessToken)
        {
            try
            {
                UserInfoByTokenDto userInfo = await _tokenService.UserInfoByTokenAsync(accessToken);
                if (userInfo == null) return new Response
                {
                    StatusCode = 500,
                    Message = "Access Token is not valid!!!!!!!!"
                };

                if (string.Equals(userInfo.User.RefreshToken, refreshToken, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _tokenService.TokenExpired(refreshToken))
                    {
                        return new Response
                        {
                            StatusCode = 200,
                            Message = await _tokenService.CreateToken(userInfo.User, userInfo.Role)
                        };
                    }
                    else
                    {
                        return new Response
                        {
                            StatusCode = 400,
                            Message = "Refresh token expired. please login again!"
                        };
                    }
                }
                else
                {
                    return new Response
                    {
                        StatusCode = 400,
                        Message = "Refresh Token is not valid!"
                    };
                }

            }
            catch (Exception e)
            {
                return new Response
                {
                    StatusCode = 500,
                    Message = e.ToString()
                };

            }
        }

        public async Task<Response> LogIn(LogInDto logInDto)
        {
            User user = await _userRepo.FindUserByUserNameAsync(logInDto.UserName);
            if (user == null) return new Response
            {
                StatusCode = 400,
                Message = $"The username {logInDto.UserName} does not exists!"
            };

            if (await _userRepo.CheckPasswordAsync(user, logInDto.Password))
            {
                IList<string> userRoles = await _userRepo.GetUserRoleAsync(user);
                if (userRoles.Count > 0)
                {
                    string accessToken = await _tokenService.CreateToken(user, userRoles[0]);
                    string refreshToken = await _tokenService.CreateToken(user);
                    await _userRepo.UpdateUserRefreshTokenAsync(user, refreshToken);

                    return new Response
                    {
                        StatusCode = 200,
                        Message = new RegisterResponse
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken,
                            User = user
                        }
                    };



                }
                else
                {
                    return new Response
                    {
                        StatusCode = 400,
                        Message = "User has no role!"
                    };
                }
            }
            else
            {
                return new Response
                {
                    StatusCode = 400,
                    Message = "Password is not correct!"
                };
            }

        }

        public async Task<Response> ResetPass(ResetPassDto resetPassDto, string accessToken)
        {
            User user = await _userRepo.FindUserByUserNameAsync(resetPassDto.UserName);
            if (user == null) return new Response
            {
                StatusCode = 404,
                Message = $"the usename {resetPassDto.UserName} does not exists!"
            };

            UserInfoByTokenDto userInfo = await _tokenService.UserInfoByTokenAsync(accessToken);

            if (userInfo == null) return new Response
            {
                StatusCode = 400,
                Message = "Access token is not valid!"
            };

            if (userInfo.User.UserName.Equals(resetPassDto.UserName, StringComparison.OrdinalIgnoreCase))
                return new Response
                {
                    StatusCode = 400,
                    Message = "Access token is not correct for this user!"
                };

            IdentityResult result = await _userRepo.ChangeUserPasswordAsync(user, resetPassDto.OldPassword, resetPassDto.NewPassword);
            if (result.Succeeded)
            {
                await _userRepo.UpdateUserAsync(user);
                return new Response
                {
                    StatusCode = 200,
                    Message = "Password has been changed successfully!"
                };
            }
            else
            {
                return new Response
                {
                    StatusCode = 400,
                    Message = result.Errors
                };
            }

        }
    }











}
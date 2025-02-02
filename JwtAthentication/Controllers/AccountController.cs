using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SecondApp.Dtos.Account;
using SecondApp.Interfaces;
using SecondApp.Models;

namespace SecondApp.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenservice;
        private readonly IUserRepository _userRepo;
        public AccountController(ITokenService tokenService, IUserRepository userRepo) { 
            _tokenservice = tokenService;
            _userRepo = userRepo;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterView([FromBody]RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            RegisterRepoRes registerResponse = await _userRepo.Register(registerDto);
            if (registerResponse.StatusCode == 200)
            {
                //Created refresh and access token for user:
                string accessToken = await _tokenservice.CreateToken(registerResponse.AppUser, role:registerDto.Role);
                string refreshToken = await _tokenservice.CreateToken(registerResponse.AppUser, isAccessToken: false);

                return StatusCode(registerResponse.StatusCode, new RegisterResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = registerResponse.AppUser
                });
            }
            else
            {

                return StatusCode(registerResponse.StatusCode, registerResponse.Message);
            }

        }
        [HttpPost]
        [Route("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshTokenView([FromBody]RefreshTokenRequestDto refreshTokenRequestDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            string? authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null) return BadRequest("Access Token is not valid");

            string? accessToken = authHeader.Substring("Bearer ".Length);

            string response = await _userRepo.RefreshToken(accessToken, refreshTokenRequestDto.RefreshToken);

            if (response.Length < 100)
            {
                return StatusCode(400, new RefreshTokenResponseDto
                {
                    Error = response,
                });
            }
            else
            {
                return StatusCode(200, new RefreshTokenResponseDto
                {
                    AccessToken = response,
                });
            }

            //return StatusCode(200, response);
        }


        [HttpPost]
        [Route("log-in")]
        public async Task<IActionResult> LoginView([FromBody]LogInDto loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RepositoryResponse loginResponse = await _userRepo.Login(loginModel);


            //if (loginResponse == null)  return 

            return StatusCode(loginResponse.StatuCode, loginResponse.Message);

        }



        [HttpPost]
        [Route("reset-password")]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null) return BadRequest("Access Token is not valid");

            string accessToken = authHeader.Substring("Bearer ".Length);

            RepositoryResponse response = await _userRepo.ResetPassword(resetDto, accessToken);



            return StatusCode(response.StatuCode, response.Message);

        }
    
    
    }
}

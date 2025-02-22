using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Dtos;
using Backend.Application.Interfaces.Account;
using Backend.Presentation.API.Dtos.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Presentation.API.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class Account : ControllerBase
    {
        private readonly IUserService _userService;
        public Account(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);
            Response response = await _userService.Register(registerDto);
            return StatusCode(response.StatusCode, response.Message);


        }


        [HttpPost]
        [Route("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            string? authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null) return BadRequest("Access Token is not valid");

            string? accessToken = authHeader.Substring("Bearer ".Length);

            Response response = await _userService.RefreshToken(refreshTokenDto.RefreshToken, accessToken);

            return StatusCode(response.StatusCode, response.Message);

        }


        [HttpPost]
        [Route("log-in")]
        public async Task<IActionResult> LogIn([FromBody] LogInDto logInDto)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);
            Response response = await _userService.LogIn(logInDto);
            return StatusCode(response.StatusCode, response.Message);

        }

        [HttpPost]
        [Route("reset-pass")]
        [Authorize]
        public async Task<IActionResult> ResetPass([FromBody] ResetPassDto resetPassDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null) return BadRequest("Access Token is not valid");

            string accessToken = authHeader.Substring("Bearer ".Length);
            Response response = await _userService.ResetPass(resetPassDto, accessToken);

            return StatusCode(Response.StatusCode, response.Message);
        }


    }
}
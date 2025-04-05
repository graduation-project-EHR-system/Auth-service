using Data.Layer.Entities;
using Data.Layer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Data.Layer.Dtos;
using UserManagementService.Error;
using UserManagementService.Helper;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthService _authService;

        public AccountController(UserManager<User> userManager, 
                                 SignInManager<User> signInManager, 
                                 IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var userDto = await _authService.LoginAync(loginDto);

            if (userDto is null)
                return Unauthorized(new ApiResponse(401, "Invalid Email Or Password"));

            return Ok(new ApiResponseWithData(200 , "Logged In Successfuly!")
            {  
                Data = userDto 
            });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            if (string.IsNullOrEmpty(refreshTokenDto.RefreshToken))
                return BadRequest("Token is required!");

            var result = await _authService.LogoutAsync(refreshTokenDto.RefreshToken);

            if (!result)
                return BadRequest(new ApiResponse(400, "There is Something Wrong"));

            return Ok(new ApiResponse(200 , "Logged out Successfuly!"));
        }

        [HttpPost("Get-Refresh-Token")]
        public async Task<ActionResult<UserDto>> RefreshToken( [FromBody] RefreshTokenDto refreshTokenDto)
        {
            var user = await _authService.GetRefreshToken(refreshTokenDto.RefreshToken);

            if (user is null)
                return BadRequest(new ApiResponse(401 , "Invalid Token"));

            return Ok(new ApiResponseWithData(200 , "Valid Token")
            {
                Data = user 
            });
        }

        [HttpGet("get-user")]
        public async Task<ActionResult<UserDto>> GetUserFromToken([FromHeader] string tokenRequest)
        {
            if (string.IsNullOrEmpty(tokenRequest) || !tokenRequest.StartsWith("Bearer "))
            {
                return BadRequest("Invalid token");
            }

            var token = tokenRequest.Substring("Bearer ".Length);
            var userDto = await _authService.MeAync(token);

            if (userDto is null)
            {
                return Unauthorized(new ApiResponse(401 , "Email not found in token"));
            }

            return Ok(new ApiResponseWithData(200, "User is found Successfuly!")
            {
                Data = userDto
            });
        }

        [HttpPost("request-reset-password")]
        public async Task<ActionResult> RequestResetPassword([FromBody] RequestResetPasswordDto requestResetPasswordDto)
        {
            var success = await _authService.RequestPasswordResetAsync(requestResetPasswordDto);
            if (!success)
                return NotFound(new ApiResponse(404 , "User not found."));

            return Ok(new ApiResponse(200, "Password reset email sent.") );
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<IdentityResult>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result)
                return IdentityResult.Failed(new IdentityError { Description = "Password reset failed." });

            return Ok(new ApiResponse(200, "Password has been reset successfully.") );
        }

    }
}

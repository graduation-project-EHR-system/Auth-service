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
                return BadRequest(new ApiResponse(400));

            return Ok(new ApiResponse(200 , "Logged out Successfuly!"));
        }

        [HttpPost("GetRefreshToken")]
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
        
    }
}

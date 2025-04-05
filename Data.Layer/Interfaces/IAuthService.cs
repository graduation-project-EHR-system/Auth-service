using Data.Layer.Dtos;
using Data.Layer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Layer.Interfaces
{
    public interface IAuthService
    {
        public Task<string> GenerateTokenAsync(User user, UserManager<User> userManager);

        public Task<UserDto> LoginAync(LoginDto loginDto);

        public Task<UserDto> MeAync(string token);

        public Task<bool> LogoutAsync(string refreshToken);

        public Task<UserDto> GetRefreshToken(string refreshToken);

        Task<bool> RequestPasswordResetAsync(RequestResetPasswordDto requestResetPasswordDto);

        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    }
}

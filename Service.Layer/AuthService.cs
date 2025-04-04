using Data.Layer.Entities;
using Data.Layer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Data.Layer.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;


namespace Service.Layer
{

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AuthService(IConfiguration configuration, 
                            UserManager<User> userManager,
                            IMapper mapper)
        {
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<string> GenerateTokenAsync(User user, UserManager<User> userManager)
        {
            var authClaims = new List<Claim>
            {
                new Claim("Email", user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("NationalId" , user.NationalId),
                new Claim("Name" , user.DisplayName),
                new Claim("ID" , user.Id),
            };

            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                authClaims.Add(new Claim("Role", role));
            }
            
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecurityKey"]));

            var token = new JwtSecurityToken(
                audience: _configuration["JWT:ValidAudience"],
                issuer: _configuration["JWT:ValidIssuer"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:DurationInMinutes"] ?? "2")),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

       

        public async Task<UserDto> LoginAync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return null;


            var userDTO = _mapper.Map<UserDto>(user);   

            userDTO.Token = await GenerateTokenAsync(user, _userManager);

            userDTO.Role = (await _userManager.GetRolesAsync(user)).First();


            if (user.RefreshTokens.Any(rt => rt.IsActive))
            {
                var activerefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.IsActive);
                userDTO.RefreshToken = activerefreshToken.Token;
                userDTO.RefreshTokenExpiration = activerefreshToken.ExpiredOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                userDTO.RefreshToken = refreshToken.Token;
                userDTO.RefreshTokenExpiration = refreshToken.ExpiredOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            return userDTO;
        }


        public async Task<UserDto> GetRefreshToken(string refreshToken)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

            if (user == null)
                return null;

            var reftoken = user.RefreshTokens.Single(t => t.Token == refreshToken);

            if (!reftoken.IsActive)
                return null;

            reftoken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);


            var userDTO = _mapper.Map<UserDto>(user);


            userDTO.Token = await GenerateTokenAsync(user, _userManager);
            userDTO.RefreshToken = newRefreshToken.Token;
            userDTO.RefreshTokenExpiration = newRefreshToken.ExpiredOn;
            userDTO.Role = (await _userManager.GetRolesAsync(user)).First();


            return userDTO;
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

            if (user == null)
                return false;

            var reftoken = user.RefreshTokens.Single(t => t.Token == refreshToken);

            if (!reftoken.IsActive)
                return false;

            reftoken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = WebEncoders.Base64UrlEncode(randomNumber),
                ExpiredOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow,
            };
        }

        public async Task<UserDto> MeAync(string token)
        {
            
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);

            
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Email" || c.Type == "email").Value;

            var user = await _userManager.FindByEmailAsync(emailClaim);

            if (user is null)
                return null;

            var userDTO = _mapper.Map<UserDto>(user);

            userDTO.Token = token;


            userDTO.Role = (await _userManager.GetRolesAsync(user)).First();


            if (user.RefreshTokens.Any(rt => rt.IsActive))
            {
                var activerefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.IsActive);
                userDTO.RefreshToken = activerefreshToken.Token;
                userDTO.RefreshTokenExpiration = activerefreshToken.ExpiredOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                userDTO.RefreshToken = refreshToken.Token;
                userDTO.RefreshTokenExpiration = refreshToken.ExpiredOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return userDTO;

        }
    }
}

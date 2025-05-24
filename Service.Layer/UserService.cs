using Data.Layer.Dtos;
using Data.Layer.Entities;
using Data.Layer.Helper.SendEmail;
using Data.Layer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using UserManagementService.Interfaces;

namespace Service.Layer
{

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSend _emailSender;

        public UserService(
               UserManager<User> userManager,
               ILogger<UserService> logger,
                RoleManager<IdentityRole> roleManager,
                IEmailSend emailSender
          )
        {
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        public async Task<User> HandleUserCreatedEventAsync(KafkaUserEvent userEvent)
        {
            _logger.LogInformation("Handling user created event at {time}", DateTime.UtcNow);

            var user = new User
            {
                UserName = userEvent.Body.Email.Split("@")[0],
                DisplayName = userEvent.Body.Email.Split("@")[0],
                Id = userEvent.Body.Id,
                FirstName = userEvent.Body.FirstName,
                LastName = userEvent.Body.LastName,
                Email = userEvent.Body.Email,
                PhoneNumber = userEvent.Body.PhoneNumber,
                Gender = userEvent.Body.Gender,
                NationalId = userEvent.Body.NationalId,
            };

            var generatedPassword = GenerateRandomPassword();

            try
            {
                var result = await _userManager.CreateAsync(user, generatedPassword);

                if (!result.Succeeded)
                {
                    _logger.LogError("User creation failed. Errors: {errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return null;
                }

                _logger.LogInformation("User created successfully at {time}", DateTime.UtcNow);

                await SendEmailWithPassword(user, userEvent.Body.Email, generatedPassword);

                await _userManager.AddToRoleAsync(user, userEvent.Body.Role);


                _logger.LogInformation("User's Role created successfully at {time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("User Creation Failed at {time}", DateTime.UtcNow);
                _logger.LogError(ex, "Error creating user: {message}", ex.Message);
            }

            return user;

        }

        private string GenerateRandomPassword(int length = 10)
        {
            if (length < 4)
                throw new ArgumentException("Password length must be at least 4 to meet all requirements.");

            const string upper = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*?";

            var random = new Random();

            var passwordChars = new List<char>
            {
                upper[random.Next(upper.Length)],
                lower[random.Next(lower.Length)],
                digits[random.Next(digits.Length)],
                symbols[random.Next(symbols.Length)]
            };

            string allChars = upper + lower + digits + symbols;
            for (int i = passwordChars.Count; i < length; i++)
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }

            return new string(passwordChars.OrderBy(_ => random.Next()).ToArray());
        }


        private async Task SendEmailWithPassword(User user, string emailAdderss, string generatedPassword)
        {

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var link = $"https://localhost:5259/reset-password?email={Uri.EscapeDataString(emailAdderss)}&token={Uri.EscapeDataString(token)}";


            var email = new Email
            {
                To = emailAdderss,
                Subject = "Your Account Created Successfully",
                Body = $"Your account has been created successfully. Your password is: {generatedPassword}\n" +

                        $"Click the link below to reset your password:\n{link}"
            };

            await _emailSender.SendEmailAsync(email);
        }
    }
}

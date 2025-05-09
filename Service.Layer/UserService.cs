using Data.Layer.Dtos;
using Data.Layer.Entities;
using Data.Layer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserManagementService.Interfaces;

namespace Service.Layer
{

     public class UserService : IUserService
     {
          private readonly UserManager<User> _userManager;
          private readonly ILogger<UserService> _logger;

          public UserService(
               UserManager<User> userManager,
               ILogger<UserService> logger
          ){
               _userManager = userManager;
               _logger = logger;
          }

          public async Task<User> HandleUserCreatedEventAsync(KafkaUserEvent userEvent)
          {
               _logger.LogInformation("Handling user created event at {time}", DateTime.UtcNow);

               var user = new User
               {
                    UserName = userEvent.Body.FirstName + " " + userEvent.Body.LastName,
                    Email = userEvent.Body.Email,
                    PhoneNumber = userEvent.Body.Phone,
                    PasswordHash = "password",
               };

               await _userManager.CreateAsync(user);

               _logger.LogInformation("User created successfully at {time}", DateTime.UtcNow);

               return user;
          }
     }
}

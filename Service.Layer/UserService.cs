using Data.Layer.Dtos;
using Data.Layer.Entities;
using Data.Layer.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Service.Layer
{

     public class UserService : IUserService
     {
          private readonly UserManager<User> _userManager;

          public UserService(
               UserManager<User> userManager
          ){
               _userManager = userManager;
          }
     }
}

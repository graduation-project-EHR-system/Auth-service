using Data.Layer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Layer.Contexts
{
    public static class DataSeeding
    {
        public async static Task SeedingDataAsync(UserManager<User> userManager , RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Doctor", "Receptionist", "Patient"};

            if (userManager.Users.Count() == 0)
            {
                var firstUser = new User
                {
                    FirstName = "Badry",
                    LastName = "Anas",
                    Age = 22,
                    DisplayName = "Badry Anas",
                    PhoneNumber = "01234567891",
                    Email = "badry.anas@email.com",
                    NormalizedEmail = "BADRY.ANAS@EMAIL.COM",
                    UserName = "badry.anas",
                    Address = "29 - mohamed mouaid - Salah mosque - Embabh",
                    NationalId = "12345678912340",
                    Gender = GenderOption.Male,
                    MaritalStatus = MaritalStatus.Married,
                    DateOfBirth = new DateTime(2003, 1, 26),
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(firstUser, "Pa$$w0rd");
            }

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var admin = await userManager.FindByEmailAsync("badry.anas@email.com");

            var RolesFirstUser = await userManager.GetRolesAsync(admin);

            if (admin != null &&  !RolesFirstUser.Any() )
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

        }
    }
}

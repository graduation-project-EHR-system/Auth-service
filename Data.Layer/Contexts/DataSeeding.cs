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
            string[] roles = { "Admin", "Doctor", "Patient" };

            if (userManager.Users.Count() == 0)
            {
                var firstUser = new User
                {
                    FirstName = "Badry",
                    LastName = "Anas",
                    DisplayName = "Badry Anas",
                    PhoneNumber = "01234567891",
                    Email = "badry.anas@email.com",
                    UserName = "badry.anas",
                    Address = "29 - mohamed mouaid - Salah mosque - Embabh",
                    City = "Giza",
                    Country = "Egypt",
                    PostalCode = "1234",
                    NationalId = "",
                    DateOfBirth = new DateTime(2003, 1, 26)
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

            if (admin != null && await userManager.GetRolesAsync(admin) is null )
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

        }
    }
}

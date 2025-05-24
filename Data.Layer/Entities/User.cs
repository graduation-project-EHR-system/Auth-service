using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Layer.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalId { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string DisplayName { get; set; }


        // Enums

        public MaritalStatus? MaritalStatus { get; set; }
        public GenderOption Gender { get; set; }


        // Dates

        public DateTime?  DateOfBirth { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;



        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}

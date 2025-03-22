using Data.Layer.Entities;
using System.ComponentModel.DataAnnotations;

namespace Data.Layer.Dtos
{
    public class UserDto
    {

        public string Id { get; set; }
        
        [Required(ErrorMessage = "Display Name is required.")]

        [MaxLength(80, ErrorMessage = "Display Name cannot exceed 50 characters.")]
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string? RefreshToken { get; set; }


        [Required(ErrorMessage = "Marital status is required.")]
        [EnumDataType(typeof(MaritalStatus), ErrorMessage = "Invalid Marital Status.")]
        public MaritalStatus MaritalStatus { get; set; }



        [Required(ErrorMessage = "Gender is required.")]
        [EnumDataType(typeof(GenderOption), ErrorMessage = "Invalid Gender Option.")]
        public GenderOption Gender { get; set; }



        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }



        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }
        

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "National ID is required.")]
        [RegularExpression("^[0-9]{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(50, ErrorMessage = "Address cannot exceed 50 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        public string Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime RefreshTokenExpiration { get; set; }
    }
}

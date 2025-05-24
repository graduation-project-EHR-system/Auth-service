
using Data.Layer.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Layer.Dtos
{
     public class KafkaUserEvent
     {
          [JsonPropertyName("timestamp")]
          public DateTime Timestamp { get; set; }

          [JsonPropertyName("body")]
          public UserBody Body { get; set; }

          public class UserBody
          {
               [JsonPropertyName("id")]
               public string Id { get; set; }

               [JsonPropertyName("firstName")]
               public string FirstName { get; set; }

               [JsonPropertyName("lastName")]
               public string LastName { get; set; }

               [JsonPropertyName("email")]
               public string Email { get; set; }

               [JsonPropertyName("phone")]
               public string PhoneNumber { get; set; }

               [JsonPropertyName("role")]
               public string Role { get; set; }

               [JsonPropertyName("gender")]
               public GenderOption Gender { get; set; }

               [JsonPropertyName("nationalId")]
               public string NationalId { get; set; }

        }
     }
}

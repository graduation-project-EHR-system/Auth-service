
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
               public string Phone { get; set; }

               [JsonPropertyName("type")]
               public string Type { get; set; }
          }
     }
}

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ToolKit.Entities
{
    public class User
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("access")]
        public string Token { get; set; }
        [JsonPropertyName("role")]
        public IEnumerable<string> Role { get; set; }
        public string ReNewToken { get; set; }
    }
}
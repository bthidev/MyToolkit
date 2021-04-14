using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ToolKit.Entities
{
    public class User
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }
        public string Token { get; set; }
        [JsonPropertyName("role")]
        public IEnumerable<string> Role { get; set; }
        public string ReNewToken { get; set; }
    }
}
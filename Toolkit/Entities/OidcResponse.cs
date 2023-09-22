using System.Text.Json.Serialization;

namespace Toolkit.Entities
{
    public class OidcResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}

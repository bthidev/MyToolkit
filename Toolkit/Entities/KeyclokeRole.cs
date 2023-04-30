using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Toolkit.Entities
{
    public class KeyclokeRole
    {
        [JsonPropertyName("roles")]
        public IEnumerable<string> Roles { get; set; }
    }
}
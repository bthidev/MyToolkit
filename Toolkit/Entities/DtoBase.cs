using System;
using System.Text.Json.Serialization;

namespace ToolKit.Entities
{
    public abstract class DtoBase
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }
    }
}

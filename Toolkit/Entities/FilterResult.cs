using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ToolKit.Entities
{
    public class FilterResult<TDto>
    {
        [JsonPropertyName("items")]
        public IEnumerable<TDto> Items { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int Total { get; set; }
    }
}
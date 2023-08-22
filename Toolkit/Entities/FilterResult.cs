using System.Collections.Generic;

namespace ToolKit.Entities
{
    public class FilterResult<TDto>
    where TDto : DtoBase
    {
        public IEnumerable<TDto> Items { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int Total { get; set; }
    }
}
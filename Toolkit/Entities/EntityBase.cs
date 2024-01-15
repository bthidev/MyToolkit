using System;
using System.ComponentModel.DataAnnotations;

namespace ToolKit.Entities
{
    public class EntityBase
    {
        [Key]
        public Guid Id { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}
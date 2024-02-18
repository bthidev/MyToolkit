using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ToolKit.Entities;

namespace Toolkit.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class UserEntity : EntityBase
    {
        [Required]
        public string Email { get; set; }
    }
}

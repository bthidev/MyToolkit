using System.ComponentModel.DataAnnotations;

namespace ToolKit.Entities
{
    public class AuthenticateModel
    {
        [Required] public string username { get; set; }

        [Required] public string password { get; set; }
    }
}
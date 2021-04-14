using System.ComponentModel.DataAnnotations;

namespace ToolKit.Entities
{
    public class AuthenticateModel
    {
        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }
    }
}
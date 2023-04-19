using Microsoft.AspNetCore.Authentication;

namespace ToolKit.Services
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string AuthUrl { get; set; }
    }
}

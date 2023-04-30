using System;
using System.Linq;
using System.Security.Claims;

namespace ToolKit.Extention
{
    public static class ClaimsExtentions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            var claims = user.Claims.ToList();
            return claims?.
                    FirstOrDefault(x =>
                        x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }
}
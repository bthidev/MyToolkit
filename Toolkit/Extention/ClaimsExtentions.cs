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
#pragma warning disable S2589 // Boolean expressions should not be gratuitous
            return claims?.
                    Find(x =>
                        x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
        }
    }
}
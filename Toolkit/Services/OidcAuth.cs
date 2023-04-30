using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Toolkit.Entities;

namespace Toolkit.Services
{
    public static class OidcAuth
    {
        public static void AddAuthThidev(this IServiceCollection services)
        {
            services.AddAuthentication()
                    .AddJwtBearer(x =>
                    {
                        x.MetadataAddress = "https://auth.thidev.fr/auth/realms/thidev/.well-known/openid-configuration";
                        x.RequireHttpsMetadata = false;
                        x.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false
                        };
                    });
            services.AddAuthorization(o =>
            {
                o.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                o.AddPolicy("admins", policy => policy.RequireAssertion(context =>
                            context.User.HasClaim(claim => claim.Type == "realm_access" && JsonSerializer.Deserialize<KeyclokeRole>(claim.Value).Roles.Contains("admin"))));
            });
        }
    }
}
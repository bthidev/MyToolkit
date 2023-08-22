using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Toolkit.Services
{
    public static class OidcAuth
    {
        public static void AddAuthThidev(this IServiceCollection services, string authUrl, string adminRole)
        {
            services.AddAuthentication()
                    .AddJwtBearer(x =>
                    {
                        x.MetadataAddress = authUrl;
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

                o.AddPolicy(adminRole, policy => policy.RequireAssertion(context => NewMethod(adminRole, context)));
            });
        }

        private static bool NewMethod(string adminRole, AuthorizationHandlerContext context)
        {
            return context.User.HasClaim(claim => claim.Type == "groups" && claim.Value.Contains(adminRole));
        }
    }
}
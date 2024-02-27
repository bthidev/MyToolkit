using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Toolkit.DAL;

namespace Toolkit.Services
{
    public static class OidcAuth
    {
        public static void AddAuthThidev(this IServiceCollection services, string authUrl, string audience)
        {
            services.AddScoped<UserRepository>();
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(x =>
                    {
                        x.Authority = authUrl;
                        x.Audience = audience;
                        x.MetadataAddress = authUrl;
                        x.RequireHttpsMetadata = false;
                        x.Events = new JwtBearerEvents()
                        {
                            OnTokenValidated = async ctx =>
                            {
                                var userManage = ctx.HttpContext.RequestServices
                                                    .GetRequiredService<UserRepository>();
                                var temp = ctx.Principal.Identities.FirstOrDefault()?.Claims;

                                var userId = await userManage
                                                .GetOrCreateUserAsync(
                                                    temp.FirstOrDefault(e => e.Type.Contains("email"))?.Value);

                                ctx.Principal.Identities.First()
                                    .AddClaim(new Claim("userId", userId.ToString()));
                            }
                        };

                        x.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            NameClaimType = "email",
                            RoleClaimType = "groups",
                        };
                    });
            services.AddAuthorization();
        }
    }
}

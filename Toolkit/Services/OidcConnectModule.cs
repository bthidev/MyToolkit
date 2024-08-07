using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Toolkit.DAL;

namespace Toolkit.Services
{
    public static class OidcConnectModule
    {
        public static IApplicationBuilder UseOidcModule(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        public static IServiceCollection RegisterLoginModule(this IServiceCollection services, string authUrl, string clientId, string clientSecret)
        {
            services.AddScoped<UserRepository>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication((AuthenticationOptions options) =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "OpenIdConnect";
            }).AddCookie("Cookies").AddOpenIdConnect("OpenIdConnect", (OpenIdConnectOptions opt) =>
            {
                opt.SignInScheme = "Cookies";
                opt.SaveTokens = true;
                opt.GetClaimsFromUserInfoEndpoint = true;
                opt.RequireHttpsMetadata = false;
                opt.Authority = authUrl;
                opt.ClientId = clientId;
                opt.ClientSecret = clientSecret;
                opt.ResponseType = "id_token";
                opt.Scope.Add("openid");
                opt.Scope.Add("profile");
                opt.Scope.Add("offline_access");
                opt.Scope.Add("name");
                opt.Scope.Add("given_name");
                opt.Scope.Add("family_name");
                opt.Scope.Add("nickname");
                opt.Scope.Add("email");
                opt.Scope.Add("email_verified");
                opt.Scope.Add("picture");
                opt.Scope.Add("created_at");
                opt.Scope.Add("identities");
                opt.Scope.Add("phone");
                opt.Scope.Add("address");
                opt.Scope.Add("roles");
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "nickname",
                    RoleClaimType = "groups",
                    SaveSigninToken = true
                };
                opt.Events = new OpenIdConnectEvents()
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
            });
            services.AddAuthorizationCore();
            return services;
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
            });
            services.AddAuthorizationCore();
            return services;
        }
    }
}

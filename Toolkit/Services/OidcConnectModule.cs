using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opt =>
            {
                opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.SaveTokens = true;
                opt.GetClaimsFromUserInfoEndpoint = true;
                opt.RequireHttpsMetadata = false;
                opt.Authority = authUrl;
                opt.ClientId = clientId;
                opt.ClientSecret = clientSecret;
                opt.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                opt.Scope.Add("openid");
                opt.Scope.Add("profile");
                opt.Scope.Add("email");

                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "email"
                };
            });
            services.AddAuthorizationCore();

            return services;
        }
    }
}
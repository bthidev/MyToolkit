using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ToolKit.Controllers;
using ToolKit.DAL;
using ToolKit.Services;

namespace ToolKit
{
    public static class ServiceCustom
    {
        public static IServiceCollection AddJwtLogin(this IServiceCollection services, string secret)
        {
            // within this section we are configuring the authentication and setting the default scheme
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwt =>
                {
                    byte[] key = Encoding.ASCII.GetBytes(secret);
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey =
                            true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
                        IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true
                    };
                });

            services.AddSingleton<UsersManagement>();
            services.AddSingleton<IUserService>(x =>
                    new UserService(secret,
                    x.GetRequiredService<UsersManagement>()));
            services.AddMvc().AddApplicationPart(typeof(UsersController).Assembly);
            return services;
        }
    }
}
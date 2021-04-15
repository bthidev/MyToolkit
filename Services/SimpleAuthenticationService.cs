using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Toolkit.Extention;
using ToolKit.Entities;

namespace ToolKit.Services
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public BasicAuthenticationOptions()
        {
        }
        public string AuthUrl {get;set;}
    }
    public class BasicAuthenticationDefaults
    {
        public const string AuthenticationScheme = "MyScheme";
    }
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private const string _Scheme = "MyScheme";
        private readonly  string _baseUrl ;
        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _baseUrl = options.CurrentValue.AuthUrl;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorizationHeader = Request.Headers["Authorization"];
            if( string.IsNullOrEmpty(authorizationHeader)) return AuthenticateResult.NoResult();
            var http = new HttpClient();
            var token = authorizationHeader.Substring(6).Trim();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var infoServer = await http.GetJsonAsync<User>(this.Options.AuthUrl + "/users/UserInfo");
            if( infoServer is null ) return AuthenticateResult.NoResult();
            // create a ClaimsPrincipal from your header
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, infoServer.Username),
                new Claim(ClaimTypes.Name,infoServer.Username)
            };
            foreach( var role in infoServer.Role)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            AuthenticationTicket ticket = new AuthenticationTicket(claimsPrincipal,
                new AuthenticationProperties { IsPersistent = false },
                Scheme.Name
            );

            return AuthenticateResult.Success(ticket);
        }
    }
}
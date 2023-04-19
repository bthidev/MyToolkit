using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ToolKit.Entities;
using Toolkit.Extention;

namespace ToolKit.Services
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }

            var http = new HttpClient();
            var token = authorizationHeader[6..].Trim();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var infoServer = await http.GetJsonAsync<User>(Options.AuthUrl + "/users/UserInfo/").ConfigureAwait(true);
            if (infoServer is null)
            {
                return AuthenticateResult.NoResult();
            }

            // create a ClaimsPrincipal from your header
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, infoServer.Username),
                new Claim(ClaimTypes.Name, infoServer.Username),
            };
            foreach (var role in infoServer.Role)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            var ticket = new AuthenticationTicket(
                claimsPrincipal,
                new AuthenticationProperties { IsPersistent = false },
                Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
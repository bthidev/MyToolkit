using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using ToolKit.DAL;
using ToolKit.Entities;

namespace ToolKit.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);

        Task<bool> CreateUserAsync(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly string _secret;
        private readonly UsersManagement _users;

        public UserService(string secret, UsersManagement usersManagement)
        {
            _secret = secret;
            _users = usersManagement;
        }

        public async Task<bool> CreateUserAsync(string username, string password)
        {
            return await _users.CreateUserAsync(username, password).ConfigureAwait(true);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _users.GetUserAsync(username, password).ConfigureAwait(true);
            if (user == null)
            {
                return null;
            }

            var role = await _users.GetUserRoleAsync(user).ConfigureAwait(true);

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);
            var firstRole = role.FirstOrDefault() ?? Role.User;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new (ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Role, firstRole),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new User
            {
                Username = user.Email,
                Token = tokenHandler.WriteToken(token),
                Role = role,
            };
        }
    }
}

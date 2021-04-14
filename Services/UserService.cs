using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ToolKit.DAL;
using ToolKit.Entities;

namespace ToolKit.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        bool CreateUser(string username, string password);
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

        public bool CreateUser(string username, string password)
        {
            return _users.CreateUser(username, password, Role.User).GetAwaiter().GetResult();
        }

        public async Task<User> Authenticate(string username, string password)
        {
            IdentityUser user = await _users.GetUser(username, password);
            if (user == null)
            {
                return null;
            }

            System.Collections.Generic.IEnumerable<string> role = await _users.GetUserRole(user);

            // authentication successful so generate jwt token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_secret);
            string firstRole = role.FirstOrDefault();
            if (firstRole == null)
            {
                firstRole = Role.User;
            }

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Role, firstRole)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            User finalUser = new User
            {
                Username = user.Email,
                Token = tokenHandler.WriteToken(token),
                Role = role
            };
            return finalUser;
        }
    }
}
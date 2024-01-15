using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ToolKit.DAL
{
    public class UsersManagement(IServiceScopeFactory dbBin)
    {
        private readonly IServiceScopeFactory _db = dbBin;

        public async Task<bool> CreateUserAsync(string username, string password)
        {
            using var scope = _db.CreateScope();
            var um = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
            var context = scope.ServiceProvider.GetService<IdentityDbContext>();
            if (!context.Users.Any(x => x.UserName.Contains(username)))
            {
                var user = new IdentityUser
                {
                    Email = username,
                    UserName = username
                };
                var result = await um.CreateAsync(user, password).ConfigureAwait(true);
                context.SaveChanges();
                return result.Succeeded;
            }

            return false;
        }

        public async Task<IdentityUser> GetUserAsync(string username, string pass)
        {
            using var scope = _db.CreateScope();
            var context = scope.ServiceProvider.GetService<IdentityDbContext>();
            var user = context.Users.Where(w => w.Email == username).FirstOrDefault();
            var passwordValidator = new PasswordValidator<IdentityUser>();
            var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
            var result = await passwordValidator.ValidateAsync(userManager, user, pass).ConfigureAwait(true);
            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }

        public async Task<IEnumerable<string>> GetUserRoleAsync(IdentityUser user)
        {
            using var scope = _db.CreateScope();
            var context = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
            return await context.GetRolesAsync(user).ConfigureAwait(true);
        }
    }
}

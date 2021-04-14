using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ToolKit.DAL
{
    public class UsersManagement
    {
        private readonly IServiceScopeFactory _db;

        public UsersManagement(IServiceScopeFactory dbBin)
        {
            _db = dbBin;
        }

        public async Task<bool> CreateUser(string username, string password, string role)
        {
            using (IServiceScope scope = _db.CreateScope())
            {
                UserManager<IdentityUser> um = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                IdentityDbContext context = scope.ServiceProvider.GetService<IdentityDbContext>();
                if (!context.Users.Any(x => x.UserName.Contains(username)))
                {
                    IdentityUser user = new IdentityUser();
                    user.Email = username;
                    user.UserName = username;
                    var result = await um.CreateAsync(user, password);
                    context.SaveChanges();
                    return result.Succeeded;
                }

                return false;
            }
        }

        public async Task<IdentityUser> GetUser(string username, string pass)
        {
            using (IServiceScope scope = _db.CreateScope())
            {
                IdentityDbContext context = scope.ServiceProvider.GetService<IdentityDbContext>();
                IdentityUser user = context.Users.Where(w => w.Email == username).FirstOrDefault();
                PasswordValidator<IdentityUser> passwordValidator = new PasswordValidator<IdentityUser>();
                UserManager<IdentityUser> userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                IdentityResult result = await passwordValidator.ValidateAsync(userManager, user, pass);
                if (result.Succeeded)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<IEnumerable<string>> GetUserRole(IdentityUser user)
        {
            using (IServiceScope scope = _db.CreateScope())
            {
                UserManager<IdentityUser> context = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                return await context.GetRolesAsync(user);

            }
        }
    }
}
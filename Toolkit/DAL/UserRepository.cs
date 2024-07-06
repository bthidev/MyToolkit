using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Toolkit.Entities;
using ToolKit.DAL;

namespace Toolkit.DAL
{
    public class UserRepository(DbContext context, IMapper mapper) : GenericRepository<UserEntity>(context, mapper)
    {
        public async Task<Guid> GetOrCreateUserAsync(string email)
        {
            var user = await _dbSet.FirstOrDefaultAsync(e => e.Email == email).ConfigureAwait(true);
            if (user == null && !string.IsNullOrEmpty(email))
            {
                user = new UserEntity() { Id = Guid.NewGuid(), Email = email };
                _dbSet.Add(user);
                _ = await SaveAsync().ConfigureAwait(true);
                return user.Id;
            }
            if (user != null)
            {
                return user.Id;
            }

            return Guid.Empty;
        }
    }
}

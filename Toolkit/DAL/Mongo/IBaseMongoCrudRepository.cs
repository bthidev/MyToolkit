using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Toolkit.Entities;

namespace Toolkit.DAL.Mongo
{
    public interface IBaseMongoCrudRepository<TEntity>
        where TEntity : IEntity
    {
        Task CreateAsync(params TEntity[] entities);

        Task DeleteAsync(params TEntity[] entities);

        Task DeleteAsync(params Guid[] guids);

        TEntity Get(Guid guid);

        Task<IEnumerable<TEntity>> GetAsync(Guid first, Guid second, params Guid[] other);

        Task<TEntity> GetAsync(Guid guid);

        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null);

        Task UpdateAsync(params TEntity[] entities);

        Task UpdateAsync(TEntity document);
    }
}

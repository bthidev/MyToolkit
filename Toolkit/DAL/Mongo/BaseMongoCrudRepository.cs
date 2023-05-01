using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Toolkit.Entities;

namespace Toolkit.DAL.Mongo
{
    public class BaseMongoCrudRepository<TEntity> : IBaseMongoCrudRepository<TEntity>
        where TEntity : IEntity
    {
        private readonly IMongoDatabase _database;

        public BaseMongoCrudRepository(
            IDatabaseProvider databaseProvider)
        {
            _database = databaseProvider.DatabaseConnection;
        }

        protected virtual string CollectionName
        {
            get
            {
                var name = typeof(TEntity).Name;
                return char.ToLowerInvariant(name[0]) + name[1..];
            }
        }

        protected IMongoCollection<TEntity> Collection => _database.GetCollection<TEntity>(
            CollectionName,
            new MongoCollectionSettings
            {
                AssignIdOnInsert = true,
            });

        public virtual Task CreateAsync(params TEntity[] entities)
        {
            return Collection.InsertManyAsync(entities);
        }

        public virtual Task UpdateAsync(params TEntity[] entities)
        {
            return Task.WhenAll(entities.Select(e => Collection.ReplaceOneAsync(x => x.Id == e.Id, e)));
        }

        public virtual Task UpdateAsync(TEntity document)
        {
            var guid = document.Id;
            return Collection.ReplaceOneAsync(x => x.Id == guid, document);
        }

        public virtual Task DeleteAsync(params TEntity[] entities)
        {
            var guids = entities.Select(x => x.Id).ToArray();
            return DeleteAsync(guids);
        }

        public virtual Task DeleteAsync(params Guid[] guids)
        {
            return Collection.DeleteManyAsync(x => guids.Contains(x.Id));
        }

        public virtual async Task<TEntity> GetAsync(Guid guid)
        {
            var data = await (await Collection.FindAsync(x => x.Id == guid).ConfigureAwait(true))
                                .ToListAsync().ConfigureAwait(false);
            return data.FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var cursor = await Collection.FindAsync(filter ?? (_ => true)).ConfigureAwait(false);
            return await cursor.ToListAsync().ConfigureAwait(false);
        }

        public virtual TEntity Get(Guid guid)
        {
            var data = Collection.FindSync(x => x.Id == guid).ToList();
            return data.FirstOrDefault();
        }
    }
}

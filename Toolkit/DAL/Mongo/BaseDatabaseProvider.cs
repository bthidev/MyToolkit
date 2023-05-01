using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Driver;
using Toolkit.Extention;

namespace Toolkit.DAL.Mongo
{
    public abstract class BaseDatabaseProvider : IDatabaseProvider
    {
        private static readonly ConcurrentDictionary<(string, string), IMongoDatabase> _connections
            = new ConcurrentDictionary<(string, string), IMongoDatabase>();

        protected BaseDatabaseProvider(string databaseName, string connectionString)
        {
            DatabaseConnection = _connections.GetOrAdd((connectionString, databaseName), _ =>
            {
                var database = new MongoClient(connectionString).GetDatabase(databaseName);
                MyTaskExtensions.RunSync(() => ApplyMigrationsAsync(database));
                return database;
            });
        }

        public IMongoDatabase DatabaseConnection { get; }

        private async Task<IMongoDatabase> ApplyMigrationsAsync(IMongoDatabase database)
        {
            var migrationCollection = database.GetCollection<Migration>("__migrations");
            var lastMigration = migrationCollection.Find(_ => true).SortByDescending(x => x.Id).FirstOrDefault()?.Id ?? string.Empty;
            var migrations =
                GetType()
                .Assembly
                .GetAssignableTypes<IMigration>()
                .Select(x => new
                {
                    x.GetCustomAttribute<MigrationAttribute>()?.Name,
                    Instance = new Lazy<IMigration>(() => (IMigration)Activator.CreateInstance(x))
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .OrderBy(x => x.Name)
                .SkipWhile(x => string.CompareOrdinal(x.Name, lastMigration) <= 0);

            foreach (var migration in migrations)
            {
                await migration.Instance.Value.UpAsync(database).ConfigureAwait(true);
                await migrationCollection.InsertOneAsync(new Migration { Id = migration.Name! }).ConfigureAwait(true);
            }

            return database;
        }

        public class Migration
        {
            public string Id { get; set; }
        }
    }
}

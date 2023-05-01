using System.Collections.Concurrent;
using MongoDB.Driver;

namespace Toolkit.DAL.Mongo
{
    public abstract class BaseDatabaseProvider : IDatabaseProvider
    {
        private static readonly ConcurrentDictionary<(string, string), IMongoDatabase> _connections
            = new ConcurrentDictionary<(string, string), IMongoDatabase>();

        protected BaseDatabaseProvider(string databaseName, string connectionString)
        {
            DatabaseConnection = _connections.GetOrAdd((connectionString, databaseName), _ =>
                new MongoClient(connectionString).GetDatabase(databaseName));
        }

        public IMongoDatabase DatabaseConnection { get; }

        public class Migration
        {
            public string Id { get; set; }
        }
    }
}

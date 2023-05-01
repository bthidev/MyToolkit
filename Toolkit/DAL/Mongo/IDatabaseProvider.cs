using MongoDB.Driver;

namespace Toolkit.DAL.Mongo
{
    public interface IDatabaseProvider
    {
        IMongoDatabase DatabaseConnection { get; }
    }
}

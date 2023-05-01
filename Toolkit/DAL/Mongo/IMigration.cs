using System.Threading.Tasks;
using MongoDB.Driver;

namespace Toolkit.DAL.Mongo
{
    public interface IMigration
    {
        Task UpAsync(IMongoDatabase database);
    }
}
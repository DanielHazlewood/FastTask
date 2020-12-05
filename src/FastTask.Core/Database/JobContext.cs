using FastTask.Core.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace FastTask.Core.Database
{
    public class JobContext
    {
        private IMongoDatabase _mongoDatabase;
        private IMongoClient _mongoClient;
        private IMongoCollection<JobDb> _collection;
        private ILogger<JobContext> _logger = LogFactory.GetLog<JobContext>();
        public JobContext(string connectionUrl, string databaseName)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionUrl);
            _mongoClient = new MongoClient(connectionUrl);
            _mongoDatabase = _mongoClient.GetDatabase(databaseName);
            _collection = _mongoDatabase.GetCollection<JobDb>("jobs");
            var jobIndexKeys = Builders<JobDb>.IndexKeys;
            var indexKeysDefinition = new CreateIndexModel<JobDb>(jobIndexKeys.Ascending(_ => _.LastFetchedTime).Descending(_ => _.IsLocked));
            _collection.Indexes.CreateOne(indexKeysDefinition);
            
        }

        public IMongoCollection<JobDb> GetCollection()
        {
            return _collection;
        }
        
    }
}
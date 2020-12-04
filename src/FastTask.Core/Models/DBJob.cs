using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FastTask.Core.Models
{
    public class DBJob
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
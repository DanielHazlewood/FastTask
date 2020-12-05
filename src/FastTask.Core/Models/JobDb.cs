using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace FastTask.Core.Models
{
    public class JobDb
    {
        
        /// <summary>
        /// Job ID
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        /// <summary>
        /// Name of the job
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A mongodb lock, used with FindOneAndUpdate to create an atomic lock
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        /// Time this was last fetched. Used for finding expired jobs.
        /// </summary>
        public long LastFetchedTime { get; set; }
        /// <summary>
        /// A C# action serialized to a string. Will run when it is processed. 
        /// </summary>
        [BsonRepresentation(BsonType.Array)]
        public StateUpdate[] StateUpdates { get; set; }
        /// <summary>
        /// An array of filters that can be used for finding jobs. e.g can block all jobs that match a specified field
        /// </summary>
        [BsonRepresentation(BsonType.Array)]
        public string[] Filters { get; set; }
    }
}
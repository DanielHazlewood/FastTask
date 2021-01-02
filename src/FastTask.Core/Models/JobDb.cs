using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace FastTask.Core.Models
{
    public sealed class JobDb
    {
        
        /// <summary>
        /// Job ID
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        /// <summary>
        /// Current state of the job
        /// </summary>
        public JobState State { get; set; }
        /// <summary>
        /// Name of the job
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The action to perform
        /// </summary>
        public long ProcessScore { get; set; }
        /// <summary>
        /// The action to perform
        /// </summary>
        public string Action { get; set; }
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
        public List<StateUpdate> StateUpdates { get; set; } = new List<StateUpdate>();
        /// <summary>
        /// An array of filters that can be used for finding jobs. e.g can block all jobs that match a specified field
        /// </summary>
        public List<string> Filters { get; set; } = new List<string>();

        public JobDb()
        {
            
        }
        
    }
}
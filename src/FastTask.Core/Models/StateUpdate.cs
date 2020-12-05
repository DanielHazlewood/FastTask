using System;

namespace FastTask.Core.Models
{
    public class StateUpdate
    {
        /// <summary>
        /// The new state of the job
        /// </summary>
        public JobState State { get; set; }
        /// <summary>
        /// A description of the state
        /// </summary>
        public string Description { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public StateUpdate()
        {
            
        }

        public StateUpdate(JobState state, string description = "")
        {
            State = state;
            Description = description;
        }
        
    }
}
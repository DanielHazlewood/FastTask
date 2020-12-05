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
    }
}
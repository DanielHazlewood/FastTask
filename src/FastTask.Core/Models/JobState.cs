namespace FastTask.Core.Models
{
    public enum JobState : int
    {
        UNKNOWN = 0,
        SCHEDULED = 1,
        PROCESSING = 2,
        COMPLETED = 3,
        FAILED = 4
    }
}
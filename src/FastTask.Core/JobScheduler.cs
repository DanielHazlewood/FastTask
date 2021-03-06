using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FastTask.Core.Database;
using FastTask.Core.Models;

namespace FastTask.Core
{
    public static class JobScheduler
    {
        private static JobContext _jobContext;

        public static void Setup(JobContext jobContext)
        {
            _jobContext = jobContext;
        }
        
        public static async Task<bool> Schedule(Expression<Action> action, DateTime scheduledTime, string name)
        {
            var job = new JobDb {Name = name,};
            job.Action = ExpressionHelper.ToString(action);
            job.StateUpdates.Add(new StateUpdate(JobState.SCHEDULED, ""));
            await _jobContext.GetCollection().InsertOneAsync(job);
            return true;
        }
        
    }
}
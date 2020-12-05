using System.Threading;
using System.Threading.Tasks;
using FastTask.Core.Database;
using MongoDB.Bson.Serialization;

namespace FastTask.Core
{
    public class WorkerExecutor : Worker
    {
        private JobContext _jobContext;
        public WorkerExecutor(JobContext jobContext, CancellationToken cancelToken) : base(cancelToken)
        {
            _jobContext = jobContext;
        }

        public override Task<bool> DoWork()
        {
            var builder = Builders<JobDb>();
            var searchFilter = Filte
            await _jobContext.GetCollection().FindOneAndUpdateAsync(Builders)
        }
    }
}
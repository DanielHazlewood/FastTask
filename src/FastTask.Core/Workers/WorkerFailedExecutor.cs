using System;
using System.Threading;
using System.Threading.Tasks;
using FastTask.Core.Database;
using FastTask.Core.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace FastTask.Core
{
    public class WorkerFailedExecutor : Worker
    {
        private static TimeSpan _timeBetweenJobs = TimeSpan.FromMilliseconds(10);
        private JobContext _jobContext;
        private ILogger _logger = LogFactory.GetLog<WorkerScheduledExecutor>();
        public WorkerFailedExecutor(JobContext jobContext, CancellationToken cancelToken) : base(cancelToken)
        {
            _jobContext = jobContext;
        }

        public override async Task<bool> DoWork()
        {
            JobDb job = null;
            while (_cancelToken.IsCancellationRequested == false)
            {
                long processScore = (DateTime.UtcNow - TimeSpan.FromSeconds(60)).Ticks;
                var filter = Builders<JobDb>.Filter.Eq(_ => _.State, JobState.PROCESSING) &
                             Builders<JobDb>.Filter.Eq(_ => _.IsLocked, true) &
                             Builders<JobDb>.Filter.Lte(_ => _.ProcessScore, processScore);
                var updateDefinition = Builders<JobDb>.Update.Set(_ => _.IsLocked, false)
                    .Set(_ => _.LastFetchedTime, processScore).Set(_ => _.State, JobState.FAILED)
                    .AddToSet(_ => _.StateUpdates, new StateUpdate(JobState.PROCESSING));
                try
                {
                    job = await _jobContext.GetCollection().FindOneAndUpdateAsync(filter, updateDefinition);
                    if (job == null)
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    _logger.LogError($"Job({job.Id}) timed out and failed");
                }
                catch (Exception e)
                {
                    if (job != null)
                    {
                        await _jobContext.GetCollection().UpdateOneAsync(Builders<JobDb>.Filter.Eq(_ => _.Id, job.Id),
                            Builders<JobDb>.Update.Set(_ => _.IsLocked, false)
                                .Set(_ => _.State, JobState.FAILED)
                                .AddToSet(_ => _.StateUpdates, new StateUpdate(JobState.FAILED, e.Message)));
                    }

                    _logger.LogError(e.Message);
                }

                await Task.Delay(_timeBetweenJobs);
            }

            return true;
        }
    }
}
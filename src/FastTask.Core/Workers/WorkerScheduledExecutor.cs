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
    public class WorkerScheduledExecutor : Worker
    {
        private static TimeSpan _timeBetweenJobs = TimeSpan.FromMilliseconds(10);
        private JobContext _jobContext;
        private ILogger _logger = LogFactory.GetLog<WorkerScheduledExecutor>();
        public WorkerScheduledExecutor(JobContext jobContext, CancellationToken cancelToken) : base(cancelToken)
        {
            _jobContext = jobContext;
        }

        public override async Task<bool> DoWork()
        {
            JobDb job = null;
            while (_cancelToken.IsCancellationRequested == false)
            {
                CancellationTokenSource jobToken = new CancellationTokenSource();
                long processScore = DateTime.UtcNow.Ticks;
                var filter = Builders<JobDb>.Filter.Eq(_ => _.State, JobState.SCHEDULED) &
                             Builders<JobDb>.Filter.Eq(_ => _.IsLocked, false) &
                             Builders<JobDb>.Filter.Lte(_ => _.ProcessScore, processScore);
                var updateDefinition = Builders<JobDb>.Update.Set(_ => _.IsLocked, true)
                    .Set(_ => _.LastFetchedTime, processScore).Set(_ => _.State, JobState.PROCESSING)
                    .AddToSet(_ => _.StateUpdates, new StateUpdate(JobState.PROCESSING));
                try
                {
                    job = await _jobContext.GetCollection().FindOneAndUpdateAsync(filter, updateDefinition);
                    if (job == null)
                    {
                        await Task.Delay(100);
                        continue;
                    }
                    // process the job
                    _logger.LogInformation($"Processing started for job ({job.Id}) {job.Name}");
                    // Simulate a job processing until we have something real to go inside!
                    var jobTask = Task.Run(async () =>
                    {
                        await Task.Delay(10);
                    }, jobToken.Token);
                    
                    // Loop through the job while it's processing so we can update it's heartbeat
                    while (jobTask.IsCompleted == false)
                    {
                        // If it's been more than X seconds since last heartbeat, update the heartbeat.
                        if (processScore < DateTime.UtcNow.AddSeconds(15).Ticks)
                        {
                            processScore = DateTime.UtcNow.AddSeconds(15).Ticks;
                            filter = Builders<JobDb>.Filter.Eq(_ => _.Id, job.Id) &
                                     Builders<JobDb>.Filter.Eq(_ => _.State, JobState.PROCESSING) &
                                     Builders<JobDb>.Filter.Eq(_ => _.IsLocked, true);
                            updateDefinition = Builders<JobDb>.Update.Set(_ => _.LastFetchedTime, processScore);
                            var jobHeartbeat = await _jobContext.GetCollection().FindOneAndUpdateAsync(filter, updateDefinition);
                            // If the state isn't processing, and locked, then something has changed it.
                            // So we'll cancel the job and throw an exception.
                            if (jobHeartbeat == null)
                            {
                                jobToken.Cancel();
                                throw new Exception($"Job {job.Id} state has changed while doing heartbeat");
                            }
                            processScore = DateTime.UtcNow.Ticks;
                        }
                        
                        await Task.Delay(10, _cancelToken);
                    }

                    if (jobTask.IsCompletedSuccessfully == false)
                    {
                        throw jobTask.Exception.InnerException;
                    }
                    
                    // Update the db model to mark the job as completed
                    filter = Builders<JobDb>.Filter.Eq(_ => _.State, JobState.PROCESSING) &
                             Builders<JobDb>.Filter.Eq(_ => _.IsLocked, true);
                    updateDefinition = Builders<JobDb>.Update.Set(_ => _.IsLocked, false)
                        .Set(_ => _.LastFetchedTime, processScore).Set(_ => _.State, JobState.COMPLETED)
                        .AddToSet(_ => _.StateUpdates, new StateUpdate(JobState.COMPLETED));
                    job = await _jobContext.GetCollection().FindOneAndUpdateAsync(filter, updateDefinition);
                    // If we failed to mark it as completed, something has failed.
                    if (job == null)
                    {
                        throw new Exception(
                            $"Job {job.Id} state has changed since the processing of it. Something may have gone wrong");
                    }
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
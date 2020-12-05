using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FastTask.Core
{
    public static class WorkerScheduler
    {
        public static IEnumerable<Worker> UseParallelTasks(this IEnumerable<Worker> workers, CancellationToken cancelToken)
        {
            var useParallelTasks = workers as Worker[] ?? workers.ToArray();
            Parallel.ForEach(useParallelTasks, async (worker) => await worker.DoWork());
            return useParallelTasks;
        }
        /// <summary>
        /// A single thread is created for each of the workers
        /// </summary>
        /// <param name="workers"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        public static IEnumerable<Worker> UseLongRunningThreads(this IEnumerable<Worker> workers, CancellationToken cancelToken)
        {
            var dedicatedWorkers = workers as Worker[] ?? workers.ToArray();
            foreach (var worker in dedicatedWorkers)
            {
                Task.Factory.StartNew(worker.DoWork, TaskCreationOptions.LongRunning);
            }
            return dedicatedWorkers;
        }
    }
}
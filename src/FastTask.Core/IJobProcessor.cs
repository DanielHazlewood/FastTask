using System;
using System.Threading.Tasks;
using FastTask.Core.Models;

namespace FastTask.Core
{
    public interface IJobProcessor : IDisposable
    {
        Task<bool> ProcessJob(JobDb job);
    }
}
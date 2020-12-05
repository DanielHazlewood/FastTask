using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FastTask.Core
{
    public abstract class Worker
    {
        protected CancellationToken _cancelToken { get; private set; }
        public Worker(CancellationToken cancelToken)
        {
            _cancelToken = cancelToken;
        }
        public abstract Task<bool> DoWork();
    }
}
using Microsoft.Extensions.Logging;

namespace FastTask.Core
{
    public static class LogFactory
    {
        private static ILoggerFactory _loggerFactory;
        public static ILogger<T> GetLog<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }

        public static void SetLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        
        
    }
}
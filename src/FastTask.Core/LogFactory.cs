using System;
using Microsoft.Extensions.Logging;

namespace FastTask.Core
{
    public static class LogFactory
    {
        private static ILoggerFactory _loggerFactory;
        public static ILogger GetLog<T>()
        {
            return _loggerFactory.CreateLogger(typeof(T).Name);
        }

        public static void SetLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        
        
    }
}
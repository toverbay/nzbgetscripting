using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace NzbGetScripting.Logging
{
    static class Logging_Extensions
    {
        public static ILoggerFacade CreateFacade(this ILogger logger)
        {
            return CreateFacade(logger, Stopwatch.StartNew());
        }

        public static ILoggerFacade CreateFacade(this ILogger logger, Stopwatch stopwatch)
        {
            return new LoggerFacade(logger, stopwatch);
        }
    }
}

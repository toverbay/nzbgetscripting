using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NzbGetScripting
{
    static class Logging_Extensions
    {
        public static ILoggerFacade CreateFacade(this ILogger logger)
        {
            return CreateFacade(logger, Stopwatch.StartNew());
        }

        public static ILoggerFacade CreateFacade(this ILogger logger, EventId eventId)
        {
            return CreateFacade(logger, null, EventId);
        }

        public static ILoggerFacade CreateFacade(this ILogger logger, Stopwatch stopwatch)
        {
            return new LoggerFacade(logger, stopwatch, null);
        }

        public static ILoggerFacade CreateFacade(this ILogger logger, Stopwatch stopwatch, EventId eventId)
        {
            return new LoggerFacade(logger, stopwatch, eventId);
        }
    }
}

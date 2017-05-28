using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace NzbGetScripting.Tests
{
    internal static class ILogger_Mock_Extensions
    {
        public static void LogReceived(this ILogger logger, LogLevel? logLevel = null, int? eventId = null, params object[] args)
        {
            logger.Received()
                .Log(
                    logLevel ?? Arg.Any<LogLevel>(),
                    eventId.HasValue ? Arg.Is<EventId>(e => e.Id == eventId.Value) : Arg.Any<EventId>(),
                    args == null ? Arg.Any<string[]>() : Arg.Is<string[]>(stringArgs => args.All(a => stringArgs.Contains(a.ToString(), StringComparer.OrdinalIgnoreCase))),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<string[], Exception, string>>());
        }

        public static void LogReceived<TException>(this ILogger logger, TException ex, LogLevel? logLevel = null, int? eventId = null)
        where TException : Exception
        {
            logger.Received()
                .Log(
                    logLevel ?? Arg.Any<LogLevel>(),
                    eventId.HasValue ? Arg.Is<EventId>(e => e.Id == eventId.Value) : Arg.Any<EventId>(),
                    Arg.Any<string[]>(),
                    Arg.Is<TException>(e => ex == null ? e == null : e != null),
                    Arg.Any<Func<string[], Exception, string>>());
        }
    }
}

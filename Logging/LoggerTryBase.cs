using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace NzbGetScripting.Logging
{
    abstract class LoggerTryBase
    {
        private readonly ILoggerFacade _logger;

        public LoggerTryBase(ILoggerFacade logger)
        {
            _logger = logger;
        }

        protected string ActionName { get; set; }
        protected bool IsTimed { get; set; }
        protected ILoggerFacade Logger => _logger;
        private string QuotedActionName => ActionName.ToQuotedString(defaultValue: "action");

        protected int GetEventId(int? eventId = null)
        {
            return eventId.GetValueOrDefault((int)_logger.Stopwatch.ElapsedMilliseconds);
        }

        protected bool LogException(Exception ex, int? eventId)
        {
            _logger.Log(LogLevel.Error, GetEventId(eventId), ex, "An error occurred attempting to execute {0}", QuotedActionName);
            return false;
        }

        protected void LogBeginTimed(int eventId, long elapsedMs)
        {
            _logger.Log(LogLevel.Information, eventId, "Begin executing {0} at {1:hh:mm:ss.fff}", QuotedActionName, DateTime.Now);
        }

        protected void LogEndTimed(int eventId, long elapsedMs, long totalMs)
        {
            _logger.Log(LogLevel.Information, eventId, "Execution ended for {0} after {0}ms", QuotedActionName, totalMs);
        }

        protected void LogEndTimed<T>(int eventId, T result, T defaultValue, long elapsedMs, long totalMs)
        {
            _logger.Log(LogLevel.Information, eventId, "Execution ended for {0} after {0}ms", QuotedActionName, totalMs);

            if (IsNullReference(result))
            {
                _logger.Log(LogLevel.Debug, eventId, "  result is null; returning default {0}", defaultValue?.ToStringOrDefault());
            }
            else
            {
                _logger.Log(LogLevel.Debug, eventId, "  returned [type: {0}, value: {1}]", result.GetType().Name, result.ToString());
            }
        }

        protected static bool IsNullReference<T>(T value)
        {
            return (!typeof(T).GetTypeInfo().IsValueType) && EqualityComparer<T>.Default.Equals(value, default(T));
        }
    }
}

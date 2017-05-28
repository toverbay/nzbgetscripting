using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NzbGetScripting.Logging
{
    public sealed class LoggerFacade : ILoggerFacade
    {
        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch;

        public Stopwatch Stopwatch => _stopwatch;

        public LoggerFacade(ILogger logger)
        : this(logger, Stopwatch.StartNew())
        { }

        public LoggerFacade(ILogger logger, Stopwatch stopwatch)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stopwatch = stopwatch ?? Stopwatch.StartNew();
        }

        public void Trace(object value)
        {
            Log(LogLevel.Trace, GetEventId(), null, value.ToQuotedString(), null);
        }

        public void Trace(string msg)
        {
            Log(LogLevel.Trace, GetEventId(), null, msg, null);
        }

        public void Trace(string format, params object[] args)
        {
            Log(LogLevel.Trace, GetEventId(), null, format, args);
        }

        public void Trace(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Trace, GetEventId(), ex, format, args);
        }

        public void Trace(int eventId, object value)
        {
            Log(LogLevel.Trace, GetEventId(eventId), null, value.ToQuotedString(), null);
        }

        public void Trace(int eventId, string msg)
        {
            Log(LogLevel.Trace, GetEventId(eventId), null, msg, null);
        }

        public void Trace(int eventId, string format, params object[] args)
        {
            Log(LogLevel.Trace, GetEventId(eventId), null, format, args);
        }

        public void Trace(int eventId, Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Trace, GetEventId(eventId), ex, format, args);
        }

        public void Debug(object value)
        {
            Log(LogLevel.Debug, GetEventId(), null, value.ToQuotedString(), null);
        }

        public void Debug(string msg)
        {
            Log(LogLevel.Debug, GetEventId(), null, msg, null);
        }

        public void Debug(string format, params object[] args)
        {
            Log(LogLevel.Debug, GetEventId(), null, format, args);
        }

        public void Debug(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Debug, GetEventId(), ex, format, args);
        }

        public void Debug(int eventId, object value)
        {
            Log(LogLevel.Debug, GetEventId(eventId), null, value.ToQuotedString(), null);
        }

        public void Debug(int eventId, string msg)
        {
            Log(LogLevel.Debug, GetEventId(eventId), null, msg, null);
        }

        public void Debug(int eventId, string format, params object[] args)
        {
            Log(LogLevel.Debug, GetEventId(eventId), null, format, args);
        }

        public void Debug(int eventId, Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Debug, GetEventId(eventId), ex, format, args);
        }

        public void Info(object value)
        {
            Log(LogLevel.Information, GetEventId(), null, value.ToQuotedString(), null);
        }

        public void Info(string msg)
        {
            Log(LogLevel.Information, GetEventId(), null, msg, null);
        }

        public void Info(string format, params object[] args)
        {
            Log(LogLevel.Information, GetEventId(), null, format, args);
        }

        public void Info(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Information, GetEventId(), ex, format, args);
        }

        public void Info(int eventId, object value)
        {
            Log(LogLevel.Information, GetEventId(eventId), null, value.ToQuotedString(), null);
        }

        public void Info(int eventId, string msg)
        {
            Log(LogLevel.Information, GetEventId(eventId), null, msg, null);
        }

        public void Info(int eventId, string format, params object[] args)
        {
            Log(LogLevel.Information, GetEventId(eventId), null, format, args);
        }

        public void Info(int eventId, Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Information, GetEventId(eventId), ex, format, args);
        }

        public void Warn(object value)
        {
            Log(LogLevel.Warning, GetEventId(), null, value.ToQuotedString(), null);
        }

        public void Warn(string msg)
        {
            Log(LogLevel.Warning, GetEventId(), null, msg, null);
        }

        public void Warn(string format, params object[] args)
        {
            Log(LogLevel.Warning, GetEventId(), null, format, args);
        }

        public void Warn(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Warning, GetEventId(), ex, format, args);
        }

        public void Warn(int eventId, object value)
        {
            Log(LogLevel.Warning, GetEventId(eventId), null, value.ToQuotedString(), null);
        }

        public void Warn(int eventId, string msg)
        {
            Log(LogLevel.Warning, GetEventId(eventId), null, msg, null);
        }

        public void Warn(int eventId, string format, params object[] args)
        {
            Log(LogLevel.Warning, GetEventId(eventId), null, format, args);
        }

        public void Warn(int eventId, Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Warning, GetEventId(eventId), ex, format, args);
        }

        public void Error(object value)
        {
            Log(LogLevel.Error, GetEventId(), null, value.ToQuotedString(), null);
        }

        public void Error(string msg)
        {
            Log(LogLevel.Error, GetEventId(), null, msg, null);
        }

        public void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, GetEventId(), null, format, args);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Error, GetEventId(), ex, format, args);
        }

        public void Error(int eventId, object value)
        {
            Log(LogLevel.Error, GetEventId(eventId), null, value.ToQuotedString(), null);
        }

        public void Error(int eventId, string msg)
        {
            Log(LogLevel.Error, GetEventId(eventId), null, msg, null);
        }

        public void Error(int eventId, string format, params object[] args)
        {
            Log(LogLevel.Error, GetEventId(eventId), null, format, args);
        }

        public void Error(int eventId, Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Error, GetEventId(eventId), ex, format, args);
        }

        public void Crit(object value)
        {
            Log(LogLevel.Critical, GetEventId(), null, value.ToQuotedString(), null);
        }

        public void Crit(string msg)
        {
            Log(LogLevel.Critical, GetEventId(), null, msg, null);
        }

        public void Crit(string format, params object[] args)
        {
            Log(LogLevel.Critical, GetEventId(), null, format, args);
        }

        public void Crit(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Critical, GetEventId(), ex, format, args);
        }

        public void Crit(int eventId, object value)
        {
            Log(LogLevel.Critical, GetEventId(eventId), null, value.ToQuotedString(), null);
        }

        public void Crit(int eventId, string msg)
        {
            Log(LogLevel.Critical, GetEventId(eventId), null, msg, null);
        }

        public void Crit(int eventId, string format, params object[] args)
        {
            Log(LogLevel.Critical, GetEventId(eventId), null, format, args);
        }

        public void Crit(int eventId, Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Critical, GetEventId(eventId), ex, format, args);
        }

        public void Log(LogLevel logLevel, object value)
        {
            Log(logLevel, GetEventId(), null, value.ToQuotedString(), null);
        }

        public void Log(LogLevel logLevel, string msg)
        {
            Log(logLevel, GetEventId(), null, msg, null);
        }

        public void Log(LogLevel logLevel, string format, params object[] args)
        {
            Log(logLevel, GetEventId(), null, format, args);
        }

        public void Log(LogLevel logLevel, Exception ex, string format, params object[] args)
        {
            Log(logLevel, GetEventId(), ex, format, args);
        }

        public void Log(LogLevel logLevel, int eventId, object value)
        {
            Log(logLevel, GetEventId(eventId), null, value.ToQuotedString(), null);
        }

        public void Log(LogLevel logLevel, int eventId, string msg)
        {
            Log(logLevel, GetEventId(eventId), null, msg, null);
        }

        public void Log(LogLevel logLevel, int eventId, string format, params object[] args)
        {
            Log(logLevel, GetEventId(eventId), null, format, args);
        }

        public void Log(LogLevel logLevel, int eventId, Exception ex, string format, params object[] args)
        {
            Log(logLevel, GetEventId(eventId), ex, format, args);
        }

        private void Log(LogLevel logLevel, EventId eventId, Exception ex, string format, params object[] args)
        {
            _logger.Log(logLevel, eventId, args ?? new string[] { }, null, (s, e) => string.Format(format ?? "{0}", s));

            if (ex != null)
            {
                _logger.Log(logLevel > LogLevel.Error ? LogLevel.Error : logLevel, eventId, 0, null, (s, e) =>
                    $"  {ex.GetType().Name}: {ex.Message}");

                var stackTrace = ex.StackTrace?.Split('\n').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(stackTrace))
                {
                    _logger.Log(logLevel > LogLevel.Debug ? LogLevel.Debug : logLevel,
                                eventId, 0, null,
                                (s, e) => $" {stackTrace}");
                }
            }
        }

        public ILoggerTry Try(Action action)
        {
            return new LoggerTry(this, action);
        }

        public ILoggerTry<TResult> Try<TResult>(Func<TResult> func)
        {
            return new LoggerTry<TResult>(this, func);
        }

        public ILoggerTry<TResult> Try<TResult>(Func<TResult> func, TResult defaultValue)
        {
            return new LoggerTry<TResult>(this, func, defaultValue);
        }

        public ILoggerTryAsync TryAsync(Task task)
        {
            return new LoggerTryAsync(this, task);
        }

        public ILoggerTryAsync<TResult> TryAsync<TResult>(Func<Task<TResult>> asyncFunc)
        {
            return new LoggerTryAsync<TResult>(this, asyncFunc);
        }

        public ILoggerTryAsync<TResult> TryAsync<TResult>(Func<Task<TResult>> asyncFunc, TResult defaultValue)
        {
            return new LoggerTryAsync<TResult>(this, asyncFunc, defaultValue);
        }

        private EventId GetEventId(int? eventId = null)
        {
            return new EventId(eventId.GetValueOrDefault((int)_stopwatch.ElapsedMilliseconds));
        }
    }
}
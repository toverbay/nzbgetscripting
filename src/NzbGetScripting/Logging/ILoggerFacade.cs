using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NzbGetScripting.Logging
{
    public interface ILoggerFacade : IFluentInterface
    {
        Stopwatch Stopwatch { get; }

        void Log(LogLevel logLevel, object value);
        void Log(LogLevel logLevel, string msg);
        void Log(LogLevel logLevel, string format, params object[] args);
        void Log(LogLevel logLevel, Exception ex, string format, params object[] args);
        void Log(LogLevel logLevel, int eventId, object value);
        void Log(LogLevel logLevel, int eventId, string msg);
        void Log(LogLevel logLevel, int eventId, string format, params object[] args);
        void Log(LogLevel logLevel, int eventId, Exception ex, string format, params object[] args);

        void Trace(object value);
        void Trace(string msg);
        void Trace(string format, params object[] args);
        void Trace(Exception ex, string format, params object[] args);
        void Trace(int eventId, object value);
        void Trace(int eventId, string msg);
        void Trace(int eventId, string format, params object[] args);
        void Trace(int eventId, Exception ex, string format, params object[] args);

        void Debug(object value);
        void Debug(string msg);
        void Debug(string format, params object[] args);
        void Debug(Exception ex, string format, params object[] args);
        void Debug(int eventId, object value);
        void Debug(int eventId, string msg);
        void Debug(int eventId, string format, params object[] args);
        void Debug(int eventId, Exception ex, string format, params object[] args);


        void Info(object value);
        void Info(string msg);
        void Info(string format, params object[] args);
        void Info(Exception ex, string format, params object[] args);
        void Info(int eventId, object value);
        void Info(int eventId, string msg);
        void Info(int eventId, string format, params object[] args);
        void Info(int eventId, Exception ex, string format, params object[] args);


        void Warn(object value);
        void Warn(string msg);
        void Warn(string format, params object[] args);
        void Warn(Exception ex, string format, params object[] args);
        void Warn(int eventId, object value);
        void Warn(int eventId, string msg);
        void Warn(int eventId, string format, params object[] args);
        void Warn(int eventId, Exception ex, string format, params object[] args);

        void Error(object value);
        void Error(string msg);
        void Error(string format, params object[] args);
        void Error(Exception ex, string format, params object[] args);
        void Error(int eventId, object value);
        void Error(int eventId, string msg);
        void Error(int eventId, string format, params object[] args);
        void Error(int eventId, Exception ex, string format, params object[] args);


        void Crit(object value);
        void Crit(string msg);
        void Crit(string format, params object[] args);
        void Crit(Exception ex, string format, params object[] args);
        void Crit(int eventId, object value);
        void Crit(int eventId, string msg);
        void Crit(int eventId, string format, params object[] args);
        void Crit(int eventId, Exception ex, string format, params object[] args);

        ILoggerTry Try(Action action);
        ILoggerTry<TResult> Try<TResult>(Func<TResult> func);
        ILoggerTry<TResult> Try<TResult>(Func<TResult> func, TResult defaultValue);

        ILoggerTryAsync TryAsync(Task task);
        ILoggerTryAsync<TResult> TryAsync<TResult>(Func<Task<TResult>> asyncFunc);
        ILoggerTryAsync<TResult> TryAsync<TResult>(Func<Task<TResult>> asyncFunc, TResult defaultValue);
    }
}
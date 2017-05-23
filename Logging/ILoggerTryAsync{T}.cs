using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NzbGetScripting.Logging
{
    public interface ILoggerTryActionAsync<T> : IFluentInterface
    {
        Task<T> LogAndContinue();
        Task<T> LogAndContinue(int eventId);
        Task<T> LogAndThrow();
        Task<T> LogAndThrow(int eventId);
    }

    public interface ILoggerTryNamedAsync<T> : ILoggerTryActionAsync<T>
    {
        ILoggerTryActionAsync<T> WithTiming();
    }

    public interface ILoggerTryTimedAsync<T> : ILoggerTryActionAsync<T>
    {
        ILoggerTryActionAsync<T> Named(string action);
    }

    public interface ILoggerTryAsync<T> : ILoggerTryActionAsync<T>
    {
        ILoggerTryNamedAsync<T> Named(string actionName);
        ILoggerTryTimedAsync<T> WithTiming();
    }
}

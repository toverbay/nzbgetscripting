using System.Threading.Tasks;

namespace NzbGetScripting.Logging
{
    public interface ILoggerTryActionAsync : IFluentInterface
    {
        Task LogAndContinue();
        Task LogAndContinue(int eventId);
        Task LogAndThrow();
        Task LogAndThrow(int eventId);
    }

    public interface ILoggerTryNamedAsync : ILoggerTryActionAsync
    {
        ILoggerTryActionAsync WithTiming();
    }

    public interface ILoggerTryTimedAsync : ILoggerTryActionAsync
    {
        ILoggerTryActionAsync Named(string actionName);
    }

    public interface ILoggerTryAsync : ILoggerTryActionAsync
    {
        ILoggerTryNamedAsync Named(string actionName);
        ILoggerTryTimedAsync WithTiming();
    }
}

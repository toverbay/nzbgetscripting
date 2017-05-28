using System;
using System.Threading.Tasks;

namespace NzbGetScripting.Logging
{
    sealed class LoggerTryAsync<T> : LoggerTryBase, ILoggerTryAsync<T>, ILoggerTryNamedAsync<T>, ILoggerTryTimedAsync<T>
    {
        private readonly Func<Task<T>> _action;
        private readonly T _defaultValue;

        public LoggerTryAsync(ILoggerFacade logger, Func<Task<T>> action) : this(logger, action, default(T))
        { }

        public LoggerTryAsync(ILoggerFacade logger, Func<Task<T>> action, T defaultValue) : base(logger)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _defaultValue = defaultValue;
        }

        public Task<T> LogAndContinue()
        {
            return LogAndContinueInternal(null);
        }

        public Task<T> LogAndContinue(int eventId)
        {
            return LogAndContinueInternal(eventId);
        }

        private async Task<T> LogAndContinueInternal(int? eventId)
        {
            try
            {
                var result = await (IsTimed ? TimeActionAsync(_action(), eventId) : _action());

                if (!IsNullReference(result))
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogException(ex, eventId);
            }

            return _defaultValue;
        }

        public Task<T> LogAndThrow()
        {
            return LogAndThrowInternal(null);
        }

        public Task<T> LogAndThrow(int eventId)
        {
            return LogAndThrowInternal(eventId);
        }

        private async Task<T> LogAndThrowInternal(int? eventId)
        {
            try
            {
                var result = await (IsTimed ? TimeActionAsync(_action(), eventId) : _action());

                if (!IsNullReference(result))
                {
                    return result;
                }
            }
            catch (Exception ex) when (LogException(ex, eventId))
            {
                // This block is never reached because LogException always returns false
                // and the exception is always thrown. Just a trick to log the exception 
                // without unwinding the stack.
            }

            return _defaultValue;
        }

        public ILoggerTryNamedAsync<T> Named(string actionName)
        {
            ActionName = actionName;

            return this;
        }

        public ILoggerTryTimedAsync<T> WithTiming()
        {
            IsTimed = true;

            return this;
        }

        ILoggerTryActionAsync<T> ILoggerTryNamedAsync<T>.WithTiming()
        {
            return WithTiming();
        }

        ILoggerTryActionAsync<T> ILoggerTryTimedAsync<T>.Named(string actionName)
        {
            return Named(actionName);
        }

        private Task<T> TimeActionAsync(Task<T> action, int? eventId)
        {
            return Logger.Stopwatch.TimeFunctionAsync(
                beforeAction: (elapsedMs) => LogBeginTimed(GetEventId(eventId), elapsedMs),
                action: () => action,
                afterAction: (result, elapsedMs, totalMs) => LogEndTimed(GetEventId(eventId), result, _defaultValue, elapsedMs, totalMs)
            );
        }
    }
}
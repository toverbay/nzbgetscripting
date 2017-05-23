using System;
using System.Threading.Tasks;

namespace NzbGetScripting.Logging
{
    sealed class LoggerTryAsync : LoggerTryBase, ILoggerTryAsync, ILoggerTryNamedAsync, ILoggerTryTimedAsync
    {
        private readonly Task _action;

        public LoggerTryAsync(ILoggerFacade logger, Task action) : base(logger)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public Task LogAndContinue()
        {
            return LogAndContinueInternal(null);
        }

        public Task LogAndContinue(int eventId)
        {
            return LogAndContinueInternal(eventId);
        }

        private async Task LogAndContinueInternal(int? eventId)
        {
            try
            {
                if (IsTimed)
                {
                    await TimeActionAsync(_action, eventId);
                }
                else
                {
                    await _action;
                }
            }
            catch (Exception ex)
            {
                LogException(ex, eventId);
            }
        }

        public Task LogAndThrow()
        {
            return LogAndThrowInternal(null);
        }

        public Task LogAndThrow(int eventId)
        {
            return LogAndThrowInternal(eventId);
        }

        private async Task LogAndThrowInternal(int? eventId)
        {
            try
            {
                if (IsTimed)
                {
                    await TimeActionAsync(_action, eventId);
                }
                else
                {
                    await _action;
                }
            }
            catch (Exception ex) when (LogException(ex, eventId))
            {
                // This block is never reached because LogException always returns false
                // and the exception is always thrown. Just a trick to log the exception 
                // without unwinding the stack.
            }
        }

        public ILoggerTryNamedAsync Named(string actionName)
        {
            ActionName = actionName;

            return this;
        }

        public ILoggerTryTimedAsync WithTiming()
        {
            IsTimed = true;

            return this;
        }

        ILoggerTryActionAsync ILoggerTryNamedAsync.WithTiming()
        {
            return WithTiming();
        }

        ILoggerTryActionAsync ILoggerTryTimedAsync.Named(string actionName)
        {
            return Named(actionName);
        }

        private Task TimeActionAsync(Task action, int? eventId)
        {
            return Logger.Stopwatch.TimeActionAsync(
                beforeAction: (elapsedMs) => LogBeginTimed(GetEventId(eventId), elapsedMs),
                action: action,
                afterAction: (elapsedMs, totalMs) => LogEndTimed(GetEventId(eventId), elapsedMs, totalMs)
            );
        }
    }
}

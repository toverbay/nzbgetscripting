using System;

namespace NzbGetScripting.Logging
{
    sealed class LoggerTry : LoggerTryBase, ILoggerTry, ILoggerTryNamed, ILoggerTryTimed
    {
        private readonly Action _action;

        public LoggerTry(ILoggerFacade logger, Action action) : base(logger)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void LogAndContinue(int eventId)
        {
            LogAndContinueInternal(eventId);
        }

        public void LogAndContinue()
        {
            LogAndContinueInternal(null);
        }

        private void LogAndContinueInternal(int? eventId)
        {
            try
            {
                if (IsTimed)
                {
                    TimeAction(_action, eventId);
                }
                else
                {
                    _action();
                }
            }
            catch (Exception ex)
            {
                LogException(ex, eventId);
            }
        }

        public void LogAndThrow(int eventId)
        {
            LogAndThrowInternal(eventId);
        }

        public void LogAndThrow()
        {
            LogAndThrowInternal(null);
        }

        private void LogAndThrowInternal(int? eventId)
        {
            try
            {
                if (IsTimed)
                {
                    TimeAction(_action, eventId);
                }
                else
                {
                    _action();
                }
            }
            catch (Exception ex) when (LogException(ex, eventId))
            {
                // This block is never reached because LogException always returns false
                // and the exception is always thrown. Just a trick to log the exception 
                // without unwinding the stack.
            }
        }

        public ILoggerTryNamed Named(string actionName)
        {
            ActionName = actionName;

            return this;
        }

        public ILoggerTryTimed WithTiming()
        {
            IsTimed = true;

            return this;
        }

        ILoggerTryAction ILoggerTryNamed.WithTiming()
        {
            return WithTiming();
        }

        ILoggerTryAction ILoggerTryTimed.Named(string actionName)
        {
            return Named(actionName);
        }

        private void TimeAction(Action action, int? eventId)
        {
            Logger.Stopwatch.TimeAction(
                beforeAction: (elapsedMs) => LogBeginTimed(GetEventId(eventId), elapsedMs),
                action: action,
                afterAction: (elapsedMs, totalMs) => LogEndTimed(GetEventId(eventId), elapsedMs, totalMs)
            );
        }
    }
}

using System;

namespace NzbGetScripting.Logging
{
    sealed class LoggerTry<T> : LoggerTryBase, ILoggerTry<T>, ILoggerTryNamed<T>, ILoggerTryTimed<T>
    {
        private readonly Func<T> _action;
        private readonly T _defaultValue;

        public LoggerTry(ILoggerFacade logger, Func<T> action) : this(logger, action, default(T))
        { }

        public LoggerTry(ILoggerFacade logger, Func<T> action, T defaultValue) : base(logger)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _defaultValue = defaultValue;
        }

        public T LogAndContinue(int eventId)
        {
            return LogAndContinueInternal(eventId);
        }

        public T LogAndContinue()
        {
            return LogAndContinueInternal(null);
        }

        private T LogAndContinueInternal(int? eventId)
        {
            try
            {
                var result = IsTimed ? TimeAction(_action, eventId) : _action();

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

        public T LogAndThrow(int eventId)
        {
            return LogAndThrowInternal(eventId);
        }

        public T LogAndThrow()
        {
            return LogAndThrowInternal(null);
        }

        private T LogAndThrowInternal(int? eventId)
        {
            try
            {
                var result = IsTimed ? TimeAction(_action, eventId) : _action();

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

        public ILoggerTryNamed<T> Named(string actionName)
        {
            ActionName = actionName;

            return this;
        }

        public ILoggerTryTimed<T> WithTiming()
        {
            IsTimed = true;

            return this;
        }

        ILoggerTryAction<T> ILoggerTryNamed<T>.WithTiming()
        {
            return WithTiming();
        }

        ILoggerTryAction<T> ILoggerTryTimed<T>.Named(string actionName)
        {
            return Named(actionName);
        }

        private T TimeAction(Func<T> action, int? eventId)
        {
            return Logger.Stopwatch.TimeFunction(
                beforeAction: (elapsedMs) => LogBeginTimed(GetEventId(eventId), elapsedMs),
                action: action,
                afterAction: (result, elapsedMs, totalMs) =>
                    LogEndTimed(GetEventId(eventId), result, _defaultValue, elapsedMs, totalMs)
            );
        }
    }
}

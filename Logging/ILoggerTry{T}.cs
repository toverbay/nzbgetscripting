using System;
using System.Collections.Generic;
using System.Text;

namespace NzbGetScripting.Logging
{
    public interface ILoggerTryAction<T> : IFluentInterface
    {
        T LogAndContinue();
        T LogAndContinue(int eventId);
        T LogAndThrow();
        T LogAndThrow(int eventId);
    }

    public interface ILoggerTryNamed<T> : ILoggerTryAction<T>
    {
        ILoggerTryAction<T> WithTiming();
    }

    public interface ILoggerTryTimed<T> : ILoggerTryAction<T>
    {
        ILoggerTryAction<T> Named(string actionName);
    }

    public interface ILoggerTry<T> : ILoggerTryAction<T>
    {
        ILoggerTryNamed<T> Named(string actionName);
        ILoggerTryTimed<T> WithTiming();
    }
}

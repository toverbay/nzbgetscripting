using System;
using System.Collections.Generic;
using System.Text;

namespace NzbGetScripting.Logging
{
    public interface ILoggerTryAction : IFluentInterface
    {
        void LogAndContinue();
        void LogAndContinue(int eventId);
        void LogAndThrow();
        void LogAndThrow(int eventId);
    }

    public interface ILoggerTryNamed : ILoggerTryAction
    {
        ILoggerTryAction WithTiming();
    }

    public interface ILoggerTryTimed : ILoggerTryAction
    {
        ILoggerTryAction Named(string actionName);
    }

    public interface ILoggerTry : ILoggerTryAction
    {
        ILoggerTryNamed Named(string actionName);
        ILoggerTryTimed WithTiming();
    }
}

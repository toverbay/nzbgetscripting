using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace NzbGetScripting
{
    public abstract class NzbScriptBase
    {
        protected internal string[] Args { get; internal set; }
        protected internal NzbGetScriptContext Context { get; internal set; }
        protected internal ILoggerFacade Logger { get; internal set; }

        public virtual string Name => GetType().Name;
        public virtual string ShortDescription => "My cool script";
        public virtual string LongDescription => null;
        public abstract NzbGetScriptType Type { get; }

        public abstract int Run(IEnumerable<string> args);

        protected int Success()
        {
            return NzbGetScriptContext.EXIT_CODE_SUCCESS;
        }

        protected int Failure()
        {
            return NzbGetScriptContext.EXIT_CODE_FAILURE;
        }

        protected int ParCheck()
        {
            return NzbGetScriptContext.EXIT_CODE_PAR_CHECK;
        }
    }
}

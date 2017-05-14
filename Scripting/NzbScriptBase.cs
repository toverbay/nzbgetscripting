using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace NzbGetScripting
{
    public abstract class NzbScriptBase
    {
        protected internal string[] Args { get; internal set; }
        protected internal NzbGetScriptContext Context { get; internal set; }

        public virtual string Name => GetType().Name;
        public virtual string ShortDescription => "My cool script";
        public virtual string LongDescription => null;
        public abstract NzbGetScriptType Type { get; }
    }
}

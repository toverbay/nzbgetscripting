using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NzbGetScripting
{
    public class MyScript : NzbScriptBase
    {
        public override NzbGetScriptType Type => NzbGetScriptType.PostProcessing;
        public override string Name => "TheScript";
        public override string ShortDescription => $"This is a script defined right in the {nameof(NzbGetScripting)} assembly";

        public override int Run(IEnumerable<string> args)
        {
            Logger.Trace($"Running MyScript with the following options: {string.Join(", ", args.ToArray())}");
            Logger.Debug("This is a debug-level log message (verbosity: 5)");
            Logger.Info("This is an information-level log message (verbosity: 4)");
            Logger.Warn("This is a warning-level log message (verbosity: 3)");
            Logger.Error("This is an error-level log message (verbosity: 2)");
            Logger.Crit((object)"This is a critical-level log message (verbosity: 1)");

            //throw new InvalidOperationException("This exception was thrown inside the script");

            return 0;
        }
    }
}

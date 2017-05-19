using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NzbGetScripting
{
    public class MyScript : NzbScriptBase
    {
        public override NzbGetScriptType Type => NzbGetScriptType.PostProcessing;
        public override string Name => "TheScript";
        public override string ShortDescription => $"This is a script defined right in the {nameof(NzbGetScripting)} assembly";

        public override int Run(IEnumerable<string> args)
        {
            Logger.LogTrace($"Running MyScript with the following options: {string.Join(", ", args.ToArray())}");
            Logger.LogDebug("This is a debug-level log message (verbosity: 5)");
            Logger.LogInformation("This is an information-level log message (verbosity: 4)");
            Logger.LogWarning("This is a warning-level log message (verbosity: 3)");
            Logger.LogError("This is an error-level log message (verbosity: 2)");
            Logger.LogCritical("This is a critical-level log message (verbosity: 1)");

            return 0;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace NzbGetScripting
{
    static class CommandLine_Extensions
    {
        public static Command GetCommand(this CommandSet suite, IEnumerable<string> args, out IEnumerable<string> commandArgs)
        {
            if (suite != null && args?.Count() > 0)
            {
                var extra = suite.Options.Parse(args);

                if (extra.Count > 0 && suite.Contains(extra[0]))
                {
                    commandArgs = extra.Skip(1);
                    return suite[extra[0]];
                }
            }

            commandArgs = null;
            return null;
        }
    }
}

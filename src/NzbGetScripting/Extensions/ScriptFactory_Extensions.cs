using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace NzbGetScripting
{
    static class ScriptFactory_Extensions
    {
        public static int CompileAndRunScript(this ScriptFactory factory, string pathToScript, IEnumerable<string> args)
        {
            Console.WriteLine("Sorry! Script compilation is not implemented yet.");
            return NzbGetScriptContext.EXIT_CODE_NONE;
        }

        public static int RunByNameOrDefault(this ScriptFactory factory, string scriptName, IEnumerable<string> args)
        {
            NzbScriptBase script = null;

            if (string.IsNullOrWhiteSpace(scriptName))
            {
                script = factory.FirstOrDefault();
            }
            else
            {
                factory.TryGetScript(scriptName, out script);
            }

            if (script == null)
            {
                throw new InvalidOperationException("Script not found");
            }

            try
            {
                return script.Run(args);
            }
            catch (Exception ex)
            {
                scriptName = string.IsNullOrWhiteSpace(scriptName) ? "the default script" : scriptName;

                script.Logger?.Crit(null, ex, "An error occurred while attempting to execute {0}.",
                    scriptName);

                throw;
            }
        }
    }
}

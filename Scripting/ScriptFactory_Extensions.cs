using System;
using System.Collections.Generic;
using System.Text;

namespace NzbGetScripting
{
    static class ScriptFactory_Extensions
    {
        public static int RunByName(this ScriptFactory factory, string scriptName, IEnumerable<string> args)
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

            try
            {
                return script.Run(args);
            }
            catch (Exception ex)
            {
                Console.Write("An error occurred while attempting to execute ");
                Console.Write(string.IsNullOrWhiteSpace(scriptName) ? "the default script" : scriptName);
                Console.WriteLine(":");
                Console.WriteLine(ex.Message);

                return NzbGetScriptContext.EXIT_CODE_FAILURE;
            }
        }
    }
}

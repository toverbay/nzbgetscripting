using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NzbGetScripting
{
    sealed class ScriptFactory
    {
        private readonly Lazy<IEnumerable<Type>> _scriptTypes;
        private readonly NzbGetScriptContext _scriptContext;
        private readonly IDictionary<string, NzbScriptBase> _scripts;

        public ScriptFactory(NzbGetScriptContext scriptContext, IEnumerable<NzbScriptBase> externalScripts)
        {
            _scriptContext = scriptContext ?? throw new ArgumentNullException(nameof(scriptContext));

            _scriptTypes = new Lazy<IEnumerable<Type>>(() =>
                GetType().GetTypeInfo().Assembly.GetTypes()
                    .Where(t => t.IsAssignableTo(typeof(NzbScriptBase))
                        && !t.Equals(typeof(NzbScriptBase))));

            _scripts = new Dictionary<string, NzbScriptBase>(StringComparer.OrdinalIgnoreCase);

            if ((externalScripts?.Count()).GetValueOrDefault() > 0)
            {
                foreach (var script in externalScripts)
                {
                    _scripts.Add(script.Name, script);
                }
            }
        }

        public bool TryGetScript(string name, out NzbScriptBase script)
        {
            // First, see if we already have it in our dictionary
            if (!_scripts.TryGetValue(name, out script))
            {
                // Next, loop through all the script types
                foreach (var scriptType in _scriptTypes.Value)
                {
                    // No sense in re-activating scripts that are already in the dictionary
                    if (!_scripts.Values.Any(s => s.GetType().Equals(scriptType)))
                    {
                        // Create an instance of the current type so that we can get its name
                        var thisScript = Activator.CreateInstance(scriptType, null) as NzbScriptBase;
                        if (string.Equals(thisScript?.Name, name, StringComparison.OrdinalIgnoreCase))
                        {
                            script = thisScript;
                            // The script's name matches. Set its context...
                            script.Context = _scriptContext;
                            // ...and add it to the dictionary
                            _scripts.Add(name, script);

                            break;
                        }
                    }
                }
            }

            return (script != null);
        }

        internal NzbScriptBase FirstOrDefault()
        {
            var script = _scripts.Values.FirstOrDefault();

            if (script == null)
            {
                var scriptType = _scriptTypes.Value?.FirstOrDefault();

                if (scriptType != null)
                {
                    script = Activator.CreateInstance(scriptType, null) as NzbScriptBase;
                }
            }

            if (script != null)
            {
                script.Context = _scriptContext;
            }

            return script;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NzbGetScripting
{
    public interface IDynamicConfiguration : IDynamicMetaObjectProvider, IEnumerable<KeyValuePair<string, string>>
    {
        string this[string key] { get; }
        IEnumerable<string> Keys { get; }
        bool ContainsKey(string key);
    }

    public class NzbGetScriptContext
    {
        public const int EXIT_CODE_SUCCESS = 93;
        public const int EXIT_CODE_FAILURE = 94;
        public const int EXIT_CODE_NONE = 95;
        public const int EXIT_CODE_PAR_CHECK = 92;

        private readonly IDynamicConfiguration _scriptConfig = new NzbEnvironmentConfigProvider("VSCMD_");
        private readonly IDynamicConfiguration _serverConfig = new NzbEnvironmentConfigProvider("NZBOP_");

        public NzbGetScriptContext()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        public TextWriter Out => Console.Out;
        public TextWriter Error => Console.Out;
        //public ILog Log => ???  //TODO: Implement Logging

        public dynamic ScriptConfig => _scriptConfig;
        public dynamic ServerConfig => _serverConfig;

        internal void CreateShim(IEnumerable<string> args)
        {
            Console.WriteLine("Executing SHIM command with the following args:");
            if (args?.Count() > 0)
            {
                foreach (var arg in args)
                {
                    Console.WriteLine($"\t{arg}");
                }
            }
            else
            {
                Console.WriteLine("[no args]");
            }
        }

        internal void RunScript(IEnumerable<string> args)
        {
            Console.WriteLine("Executing RUN command with the following args:");

            if (args?.Count() > 0)
            {
                var scriptName = args.First();

                foreach (var arg in args)
                {
                    Console.WriteLine($"\t{arg}");
                }
            }
            else
            {
                Console.WriteLine("[no args]");
            }
        }

        internal void NewProject(IEnumerable<string> args)
        {
            Console.WriteLine("Executing NEW command with the following args:");
            if (args?.Count() > 0)
            {
                foreach (var arg in args)
                {
                    Console.WriteLine($"\t{arg}");
                }
            }
            else
            {
                Console.WriteLine("[no args]");
            }
        }

        private class NzbEnvironmentConfigProvider : DynamicObject, IDynamicConfiguration
        {
            private readonly string _prefix;
            private static readonly Type THIS_TYPE = typeof(NzbEnvironmentConfigProvider);

            public NzbEnvironmentConfigProvider(string prefix)
            {
                _prefix = prefix;

            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (GetProperty(binder.Name, out result))
                {
                    return true;
                }

                result = this[binder.Name];

                return true;
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                try
                {
                    if (InvokeMethod(binder.Name, args, out result))
                    {
                        return true;
                    }
                }
                catch { }

                result = null;
                return false;
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                result = this[(string)indexes[0]];
                return true;
            }

            private bool GetProperty(string name, out object result)
            {
                var infos = THIS_TYPE.GetMember(name, BindingFlags.Public
                                                    | BindingFlags.GetProperty
                                                    | BindingFlags.Instance);

                if ((infos?.Length).GetValueOrDefault() > 0)
                {
                    var info = infos[0];
                    if (info.MemberType == MemberTypes.Property)
                    {
                        result = ((PropertyInfo)info).GetValue(this, null);
                        return true;
                    }
                }

                result = null;
                return false;
            }

            private bool InvokeMethod(string name, object[] args, out object result)
            {
                var infos = THIS_TYPE.GetMember(name, BindingFlags.InvokeMethod
                                                    | BindingFlags.Public
                                                    | BindingFlags.Instance);

                if ((infos?.Length).GetValueOrDefault() > 0)
                {
                    var info = infos[0] as MethodInfo;
                    result = info.Invoke(this, args);
                    return true;
                }

                result = null;
                return false;
            }

            public string this[string key] =>
                Environment.GetEnvironmentVariable($"{_prefix}{key}"?.ToUpperInvariant());

            public IEnumerable<string> Keys =>
                Environment.GetEnvironmentVariables().Keys.Cast<string>()
                    .Where(k => k.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
                    .Select(k => k.Substring(_prefix.Length));

            public bool ContainsKey(string key)
            {
                return Keys.Contains(key, StringComparer.OrdinalIgnoreCase);
            }

            IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
            {
#pragma warning disable IDE0007 // Explicit type is used here to avoid an un-necessary cast
                foreach (KeyValuePair<string, string> kvp in Environment.GetEnvironmentVariables())
#pragma warning restore IDE0007 // Use implicit type
                {
                    if (kvp.Key.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new KeyValuePair<string, string>(kvp.Key.Substring(_prefix.Length), kvp.Value);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<KeyValuePair<string, string>>)this).GetEnumerator();
            }
        }
    }
}

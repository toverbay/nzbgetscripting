namespace NzbGetScripting
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using System.Diagnostics;
    using System.Collections.Generic;
    using NzbGetScripting.Logging;

    class Program
    {
        public static string Name => typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        private static LogLevel s_LogLevel = LogLevel.Information;
        private static readonly Option SetVerbosityOption = new VerbosityOption();

        static Program()
        {
        }

        static int Main(string[] args)
        {
            // Setup DI
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var services = serviceCollection.BuildServiceProvider();

            return MainAsync(services, args)
                .GetAwaiter()
                .GetResult();
        }


        public static async Task<int> MainAsync(IServiceProvider services, params string[] args)
        {
            string scriptName = null;
            string scriptFile = null;

            var exitCode = 0;

            var logger = services.GetService<ILoggerFactory>()
                .AddConsole((msg, logLevel) => logLevel >= s_LogLevel, true)
                .CreateLogger(Name)
                .CreateFacade();

            // Command Suite
            // - new    : create a new script project
            // - run    : executes a script
            // - shim   : Generates a bash/batch file that NZBGet uses to execute the script

            var suite = new CommandSet(Name, Console.Out, Console.Error)
            {
                $"usage: {Name} COMMAND [OPTIONS]",
                { "h|?|help", h => ShowHelp() },
                SetVerbosityOption,

                new Command("new", "Create a new script project")
                {
                    //Options = new OptionSet
                    //{

                    //},
                    Run = cmdArgs => logger.Try(() =>
                        services.GetService<NzbGetScriptContext>().NewProject(cmdArgs))
                            .Named(scriptName)
                            .WithTiming()
                            .LogAndContinue()
                },

                new Command("run", "Executes a script")
                {
                    Options = new OptionSet
                    {
                        { "f|file=", pathToScript => scriptFile = pathToScript },
                        { "s|script=", name => scriptName = name },
                    },
                    Run = runArgs => logger.Try(() =>
                    {
                        var scriptFactory = services.GetService<ScriptFactory>();

                        if (IsScriptFile(scriptFile))
                        {
                            exitCode = scriptFactory.CompileAndRunScript(scriptFile, runArgs);
                        }
                        else
                        {
                            exitCode = scriptFactory.RunByNameOrDefault(scriptName, runArgs);
                        }
                    }).Named(scriptName.ToStringOrDefault("default script")).WithTiming().LogAndContinue()
                },

                new Command("shim", "Generates a bash/batch file that NZBGet uses to execute the script")
                {
                    Options = new OptionSet
                    {
                        { "f|file=", pathToScript => CompileAndRun(pathToScript) },
                        { "s|script=", name => scriptName = name},
                    },
                    Run = cmdArgs => services.GetService<NzbGetScriptContext>().CreateShim(cmdArgs)
                }
            };

            try
            {
                await Task.Run(() => suite.Run(args));
            }
            catch (Exception ex)
            {
                var command = suite.GetCommand(args, out var commandArgs);
                var stackTrace = ex.StackTrace.Split('\n').FirstOrDefault();

                logger.Error(0, "An error occurred attempting to execute the command: {0} {1}",
                    command.Name,
                    commandArgs.Count() > 0 ? string.Join(", ", commandArgs.ToArray()) : string.Empty);
                logger.Info("  {0}: {1}", ex.GetType().Name, ex.Message);

                if (!string.IsNullOrWhiteSpace(stackTrace))
                {
                    logger.Debug(" {0}", stackTrace);
                }

                return NzbGetScriptContext.EXIT_CODE_FAILURE;
            }

            var factory = services.GetService<ScriptFactory>();
            var context = services.GetService<NzbGetScriptContext>();

            if (factory.TryGetScript("MyCoolScript", out var myCoolScript))
            {
                Console.WriteLine($"MyCoolScript was found! {myCoolScript.ShortDescription}");
            }
            else
            {
                Console.WriteLine("Aww! I couldn't find MyCoolScript!");
            }

            if (factory.TryGetScript("TheScript", out var theScript))
            {
                Console.WriteLine($"TheScript was found! {theScript.ShortDescription}");
            }
            else
            {
                Console.WriteLine("Aww! I couldn't find TheScript!");
            }

            foreach (var key in context.ScriptConfig.Keys)
            {
                Console.WriteLine(key);
            }

            Console.WriteLine($"The value of ver is {context.ScriptConfig.VER}");

            if (context.ScriptConfig.ContainsKey("ARG_HOST_ARCH"))
            {
                Console.WriteLine(context.ScriptConfig["ARG_HOST_ARCH"]);
            }
            else
            {
                Console.WriteLine("oops!");
            }

            return exitCode;
        }

        private class VerbosityOption : Option
        {
            public VerbosityOption() : base("v|verb|verbosity:", "Set the verbosity of the output")
            {
            }

            protected override void OnParseComplete(OptionContext c)
            {
                SetVerbosity(c.OptionValues[0]);
                Console.WriteLine($"Verbosity level is now {s_LogLevel}");
            }
        }

        private static void SetVerbosity(string verbosity)
        {
            // First, check if the user passed in something like "verbosity=4"
            if (int.TryParse(verbosity, out int parsedInt))
            {
                // Clamp the verbosity to the available log levels
                parsedInt = Math.Max(Math.Min((int)LogLevel.None, parsedInt), (int)LogLevel.Trace);

                // The lower the verbosity, the higher the log level & vice versa
                s_LogLevel = LogLevel.None - parsedInt;
            }
            else
            {
                // Check if the user passed in something like "verbosity=warning"
                if (Enum.TryParse(verbosity, true, out LogLevel parsedLogLevel))
                {
                    s_LogLevel = parsedLogLevel;
                }
            }
        }

        private static void CompileAndRun(string pathToScript)
        {
            Console.WriteLine("Sorry! This feature is not yet implemented.");
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging();

            serviceCollection.Scan(scan => scan
                .ForAssembliesIn<NzbScriptBase>(Path.Combine(AppContext.BaseDirectory, "../../../scripts/"))
                .AddClasses(classes => classes.AssignableTo<NzbScriptBase>())
                .As<NzbScriptBase>()
                .WithTransientLifetime());

            serviceCollection.AddSingleton((ctx) => new NzbGetScriptContext());
            serviceCollection.AddSingleton((ctx) =>
            {
                return new ScriptFactory(ctx.GetService<NzbGetScriptContext>(),
                                         ctx.GetService<ILoggerFactory>(),
                                         ctx.GetServices<NzbScriptBase>());
            });

        }

        private static void ShowHelp()
        {
            Console.WriteLine($"General help for {Name}");
        }

        private static bool IsScriptFile(string pathToScript)
        {
            return File.Exists(pathToScript);
        }
    }
}

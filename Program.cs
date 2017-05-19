namespace NzbGetScripting
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    class Program
    {
        public static string Name => typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        private static LogLevel s_LogLevel = LogLevel.Warning;
        private static readonly Option SetVerbosityOption = new VerbosityOption();

        static int Main(string[] args)
        {
            try
            {
                return MainAsync(args)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return NzbGetScriptContext.EXIT_CODE_FAILURE;
            }
        }

        public static async Task<int> MainAsync(string[] args)
        {
            // Setup DI
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var services = serviceCollection.BuildServiceProvider();

            string scriptName = null;
            var exitCode = 0;

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
                    Run = cmdArgs => services.GetService<NzbGetScriptContext>().NewProject(cmdArgs)
                },
                new Command("run", "Executes a script")
                {
                    Options = new OptionSet
                    {
                        { "f|file=", pathToScript => CompileAndRun(pathToScript) },
                        { "s|script=", name => scriptName = name},
                    },
                    Run = runArgs => {
                        exitCode = services.GetService<ScriptFactory>().RunByName(scriptName, runArgs);
                    }
                },
                new Command("shim", "Generates a bash/batch file that NZBGet uses to execute the script")
                {
                    //Options = new OptionSet
                    //{

                    //},
                    Run = cmdArgs => services.GetService<NzbGetScriptContext>().CreateShim(cmdArgs)
                }
            };

            suite.Run(args);

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

            Console.ReadLine();

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

        private static int s_LoggerFactoryCount = 0;

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan => scan
                .ForAssembliesIn<NzbScriptBase>(Path.Combine(AppContext.BaseDirectory, "../../../scripts/"))
                .AddClasses(classes => classes.AssignableTo<NzbScriptBase>())
                .As<NzbScriptBase>()
                .WithTransientLifetime());

            serviceCollection.AddSingleton((ctx) => new NzbGetScriptContext());
            serviceCollection.AddSingleton((ctx) => {
                s_LoggerFactoryCount++;
                return new LoggerFactory()
                    .AddConsole(includeScopes: true, minLevel: s_LogLevel);
            });

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
    }
}

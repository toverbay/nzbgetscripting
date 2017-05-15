namespace NzbGetScripting
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Linq;

    class Program
    {
        public static string Name => typeof(Program).GetTypeInfo().Assembly.GetName().Name;

        static int Main(string[] args)
        {
            // Setup DI
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var services = serviceCollection.BuildServiceProvider();

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

            string scriptName = null;
            var exitCode = 0;

            // Command Suite
            // - new    : create a new script project
            // - run    : executes a script
            // - shim   : Generates a bash/batch file that NZBGet uses to execute the script
            var suite = new CommandSet(Name, Console.Out, Console.Error)
            {
                $"usage: {Name} COMMAND [OPTIONS]",
                { "h|?|help", v => ShowHelp() },
                new Command("new", "Create a new script project")
                {
                    //Options = new OptionSet
                    //{

                    //},
                    Run = cmdArgs => context.NewProject(cmdArgs)
                },
                new Command("run", "Executes a script")
                {
                    Options = new OptionSet
                    {
                        { "f|file=", pathToScript => CompileAndRun(pathToScript) },
                        { "s|script=", name => scriptName = name}
                    },
                    Run = runArgs => {
                        exitCode = factory.RunByName(scriptName, runArgs);
                    }
                },
                new Command("shim", "Generates a bash/batch file that NZBGet uses to execute the script")
                {
                    //Options = new OptionSet
                    //{

                    //},
                    Run = cmdArgs => context.CreateShim(cmdArgs)
                }
            };

            suite.Run(args);

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

        private static void CompileAndRun(string pathToScript)
        {
            Console.WriteLine("Sorry! This feature is not yet implemented.");
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan => scan
                .ForAssembliesIn<NzbScriptBase>(Path.Combine(AppContext.BaseDirectory, "../../../scripts/"))
                .AddClasses(classes => classes.AssignableTo<NzbScriptBase>())
                .As<NzbScriptBase>()
                .WithTransientLifetime());

            serviceCollection.AddSingleton((ctx) => new NzbGetScriptContext());
            serviceCollection.AddSingleton((ctx) =>
            {
                return new ScriptFactory(ctx.GetService<NzbGetScriptContext>(), ctx.GetServices<NzbScriptBase>());
            });
        }

        private static void ShowHelp()
        {
            Console.WriteLine($"General help for {Name}");
        }
    }
}

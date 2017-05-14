namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyModel;
    using System.IO;

    internal class TypeSourceSelector : ITypeSourceSelector, ISelector
    {
        private List<ISelector> _selectors = new List<ISelector>();

        protected List<ISelector> Selectors => _selectors;

        public IImplementationTypeSelector FromAssemblyOf<T>()
        {
            return InternalFromAssembliesOf(new[] { typeof(T).GetTypeInfo() });
        }

        public IImplementationTypeSelector FromEntryAssembly()
        {
            return FromAssemblies(Assembly.GetEntryAssembly());
        }

        public IImplementationTypeSelector FromApplicationDependencies()
        {
            return FromDependencyContext(DependencyContext.Default);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            return FromAssemblies(context.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(context))
                .Select(Assembly.Load)
                .ToArray());           
        }

        public IImplementationTypeSelector FromAssembliesOf(params Type[] types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            return InternalFromAssembliesOf(types.Select(x => x.GetTypeInfo()));
        }

        public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            return InternalFromAssembliesOf(types.Select(x => x.GetTypeInfo()));
        }

        public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies)
        {
            assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

            return InternalFromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies)
        {
            assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

            return InternalFromAssemblies(assemblies);
        }

        public IServiceTypeSelector AddTypes(params Type[] types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            return AddSelector(types);
        }

        public IServiceTypeSelector Addtypes(IEnumerable<Type> types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            return AddSelector(types);
        }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            foreach (var selector in Selectors)
            {
                selector.Populate(services, registrationStrategy);
            }
        }

        private IImplementationTypeSelector InternalFromAssembliesOf(IEnumerable<TypeInfo> typeInfos)
        {
            return InternalFromAssemblies(typeInfos.Select(t => t.Assembly));
        }

        private IImplementationTypeSelector InternalFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return AddSelector(assemblies.SelectMany(asm => asm.DefinedTypes)
                .Select(ti => ti.AsType()));
        }

        private IServiceTypeSelector AddSelector(IEnumerable<Type> types)
        {
            var selector = new ServiceTypeSelector(types);

            Selectors.Add(selector);

            return selector;
        }

        public IImplementationTypeSelector ForAssembliesIn(string assemblyPath)
        {
            return ForAssembliesIn(assemblyPath, null);
        }

        public IImplementationTypeSelector ForAssembliesIn<T>(string assemblyPath)
        {
            return ForAssembliesIn(assemblyPath, new Type[] { typeof(T) });
        }

        public IImplementationTypeSelector ForAssembliesIn(string assemblyPath, params Type[] types)
        {
            return ForAssembliesIn(assemblyPath, types.AsEnumerable());
        }

        public IImplementationTypeSelector ForAssembliesIn(string assemblyPath, IEnumerable<Type> types)
        {
            var foundAssemblies = FindAssembliesWithTypesx(assemblyPath, types)
                .ToArray();
            return InternalFromAssemblies(foundAssemblies);
        }

        private IEnumerable<Assembly> FindAssembliesWithTypesx(string pathToSearch, IEnumerable<Type> types)
        {
            if (!Directory.Exists(pathToSearch))
            {
                throw new DirectoryNotFoundException($@"The path ""{pathToSearch}"" does not exist.");
            }

            var asl = new AssemblyLoader(pathToSearch);
            var typeInfos = types.Select(t => t.GetTypeInfo());

            foreach (var item in Directory.EnumerateFiles(pathToSearch, "*.dll"))
            {
                var asm = asl.LoadFromAssemblyPath(item);
                foreach (var thisType in asm.DefinedTypes)
                {
                    foreach (var baseType in types)
                    {
                        if (baseType.IsAssignableFrom(thisType.AsType()))
                        {
                            yield return asm;
                        }
                    }
                }
            }
        }

        private class AssemblyLoader : AssemblyLoadContext
        {
            private readonly string _pathToAssemblies;

            public AssemblyLoader(string pathToAssemblies)
            {
                _pathToAssemblies = string.IsNullOrWhiteSpace(pathToAssemblies)
                    ? Directory.GetCurrentDirectory()
                    : pathToAssemblies;
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                var deps = DependencyContext.Default;
                var res = deps.CompileLibraries
                    .Where(d => d.Name.Contains(assemblyName.Name));
                if (res.Count() > 0)
                {
                    return Assembly.Load(new AssemblyName(res.First().Name));
                }
                else
                {
                    var dllFileInfo = new FileInfo($"{Path.Combine(_pathToAssemblies, assemblyName.Name)}.dll");
                    if (File.Exists(dllFileInfo.FullName))
                    {
                        return LoadFromAssemblyPath(dllFileInfo.FullName);
                    }
                }

                return Assembly.Load(assemblyName);
            }
        }
    }
}
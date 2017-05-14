namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Extensions.DependencyModel;

    interface IAssemblySelector : IFluentInterface
    {
        IImplementationTypeSelector FromEntryAssembly();

        IImplementationTypeSelector FromApplicationDependencies();

        IImplementationTypeSelector FromDependencyContext(DependencyContext context);

        IImplementationTypeSelector FromAssemblyOf<T>();

        IImplementationTypeSelector FromAssembliesOf(params Type[] types);

        IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types);

        IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies);

        IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies);

        IImplementationTypeSelector ForAssembliesIn<T>(string assemblyPath);

        IImplementationTypeSelector ForAssembliesIn(string assemblyPath, params Type[] types);

        IImplementationTypeSelector ForAssembliesIn(string assemblyPath, IEnumerable<Type> types);
    }
}

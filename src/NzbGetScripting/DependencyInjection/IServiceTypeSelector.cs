namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal interface IServiceTypeSelector : IImplementationTypeSelector
    {
        ILifeTimeSelector AsSelf();

        ILifeTimeSelector As<T>();

        ILifeTimeSelector As(params Type[] types);

        ILifeTimeSelector As(IEnumerable<Type> types);

        ILifeTimeSelector AsImplementedInterfaces();

        ILifeTimeSelector AsMatchingInterface();

        ILifeTimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter> filter);

        ILifeTimeSelector As(Func<Type, IEnumerable<Type>> selector);

        IImplementationTypeSelector UsingAttributes();

        IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy);
    }
}
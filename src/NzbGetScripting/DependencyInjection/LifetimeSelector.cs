namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class LifetimeSelector : ServiceTypeSelector, ILifeTimeSelector, ISelector
    {
        private readonly IEnumerable<TypeMap> _typeMaps;
        private ServiceLifetime? _lifetime;

        public LifetimeSelector(IEnumerable<Type> types, IEnumerable<TypeMap> typeMaps) : base(types)
        {
            _typeMaps = typeMaps;
        }

        public IImplementationTypeSelector WithSingletonLifetime()
        {
            return WithLifetime(ServiceLifetime.Singleton);
        }

        public IImplementationTypeSelector WithScopedLifetime()
        {
            return WithLifetime(ServiceLifetime.Scoped);
        }

        public IImplementationTypeSelector WithTransientLifetime()
        {
            return WithLifetime(ServiceLifetime.Transient);
        }

        public IImplementationTypeSelector WithLifetime(ServiceLifetime lifetime)
        {
            if (!Enum.IsDefined(typeof(ServiceLifetime), lifetime))
            {
                throw new ArgumentException(
                    $"Value of {nameof(lifetime)} must be one of [{string.Join(", ", Enum.GetNames(typeof(ServiceLifetime)))}].");
            }

            _lifetime = lifetime;

            return this;
        }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            _lifetime = _lifetime ?? ServiceLifetime.Transient;

            registrationStrategy = registrationStrategy ?? RegistrationStrategy.Append;

            foreach (var typeMap in _typeMaps)
            {
                foreach (var serviceType in typeMap.ServiceTypes)
                {
                    var implementationType = typeMap.ImplementationType;

                    if (!implementationType.IsAssignableTo(serviceType))
                    {
                        throw new InvalidOperationException(
                            $@"Type ""{implementationType.FullName}"" is not assignable to ""{serviceType.FullName}"".");
                    }

                    var descriptor = new ServiceDescriptor(serviceType, implementationType, _lifetime.Value);

                    registrationStrategy.Apply(services, descriptor);
                }
            }
        }
    }
}
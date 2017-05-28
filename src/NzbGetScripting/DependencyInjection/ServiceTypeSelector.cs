namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    internal class ServiceTypeSelector : ImplementationTypeSelector, IServiceTypeSelector, ISelector
    {
        public ServiceTypeSelector(IEnumerable<Type> types) : base(types)
        {
        }

        private RegistrationStrategy RegistrationStrategy { get; set; }

        public ILifeTimeSelector AsSelf()
        {
            return As(t => new[] { t });
        }

        public ILifeTimeSelector As<T>()
        {
            return As(typeof(T));
        }

        public ILifeTimeSelector As(params Type[] types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            return As(types.AsEnumerable());
        }

        public ILifeTimeSelector As(IEnumerable<Type> types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            return AddSelector(Types.Select(t => new TypeMap(t, types)));
        }

        public ILifeTimeSelector AsImplementedInterfaces()
        {
            return AsTypeInfo(t => t.ImplementedInterfaces);
        }

        public ILifeTimeSelector AsMatchingInterface()
        {
            return AsMatchingInterface(null);
        }

        public ILifeTimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter> action)
        {
            return AsTypeInfo(t => t.FindMatchingInterface(action));
        }

        public ILifeTimeSelector As(Func<Type, IEnumerable<Type>> selector)
        {
            selector = selector ?? throw new ArgumentNullException(nameof(selector));

            return AddSelector(Types.Select(t => new TypeMap(t, selector(t))));
        }

        public IImplementationTypeSelector UsingAttributes()
        {
            var selector = new AttributeSelector(Types);

            Selectors.Add(selector);

            return this;
        }

        public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy)
        {
            registrationStrategy = registrationStrategy ?? throw new ArgumentNullException(nameof(registrationStrategy));

            RegistrationStrategy = registrationStrategy;

            return this;
        }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            if (Selectors.Count == 0)
            {
                AsSelf();
            }

            var strategy = RegistrationStrategy ?? registrationStrategy;

            foreach (var selector in Selectors)
            {
                selector.Populate(services, strategy);
            }
        }

        private ILifeTimeSelector AddSelector(IEnumerable<TypeMap> types)
        {
            var selector = new LifetimeSelector(Types, types);

            Selectors.Add(selector);

            return selector;
        }

        private ILifeTimeSelector AsTypeInfo(Func<TypeInfo, IEnumerable<Type>> selector)
        {
            return As(t => selector(t.GetTypeInfo()));
        }
    }
}
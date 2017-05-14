namespace NzbGetScripting
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    internal abstract class RegistrationStrategy
    {
        public static RegistrationStrategy Skip => new SkipRegistrationStrategy();
        public static RegistrationStrategy Append => new AppendRegistrationStrategy();

        public static RegistrationStrategy Replace()
        {
            return Replace(ReplacementBehavior.Default);
        }

        public static RegistrationStrategy Replace(ReplacementBehavior behavior)
        {
            return new ReplaceRegistrationStrategy(behavior);
        }

        public abstract void Apply(IServiceCollection services, ServiceDescriptor descriptor);

        private sealed class SkipRegistrationStrategy : RegistrationStrategy
        {
            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
            {
                services.TryAdd(descriptor);
            }
        }

        private sealed class AppendRegistrationStrategy : RegistrationStrategy
        {
            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
            {
                services.Add(descriptor);
            }
        }

        private sealed class ReplaceRegistrationStrategy : RegistrationStrategy
        {
            private readonly ReplacementBehavior _behavior;

            public ReplaceRegistrationStrategy(ReplacementBehavior behavior)
            {
                _behavior = behavior;
            }

            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
            {
                var behavior = _behavior == ReplacementBehavior.Default
                    ? ReplacementBehavior.ServiceType
                    : _behavior;

                if (behavior.HasFlag(ReplacementBehavior.ServiceType))
                {
                    for (var i = services.Count - 1; i >= 0; i--)
                    {
                        if (services[i].ServiceType == descriptor.ServiceType)
                        {
                            services.RemoveAt(i);
                        }
                    }
                }

                if (behavior.HasFlag(ReplacementBehavior.ImplementationType))
                {
                    for (var i = services.Count - 1; i >= 0; i--)
                    {
                        if (services[i].ImplementationType == descriptor.ImplementationType)
                        {
                            services.RemoveAt(i);
                        }
                    }
                }

                services.Add(descriptor);
            }
        }
    }

}
// Cribbed from https://github.com/khellang/Scrutor/blob/master/src/Scrutor/RegistrationStrategy.cs

namespace NzbGetScripting
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    static class ServiceCollection_Extensions
    {
        public static IServiceCollection Scan(this IServiceCollection services, Action<ITypeSourceSelector> action)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            action = action ?? throw new ArgumentNullException(nameof(action));

            var selector = new TypeSourceSelector();

            action(selector);

            return Populate(services, selector, RegistrationStrategy.Append);
        }

        private static IServiceCollection Populate(this IServiceCollection services, ISelector selector, RegistrationStrategy registrationStrategy)
        {
            selector.Populate(services, registrationStrategy);
            return services;
        }
    }
}

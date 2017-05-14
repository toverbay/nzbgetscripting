namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    internal class AttributeSelector : ISelector
    {
        private readonly IEnumerable<Type> _types;

        public AttributeSelector(IEnumerable<Type> types)
        {
            _types = types;
        }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            registrationStrategy = registrationStrategy ?? RegistrationStrategy.Append;

            foreach (var type in _types)
            {
                var typeInfo = type.GetTypeInfo();

                var attributes = typeInfo.GetCustomAttributes<ServiceDescriptorAttribute>().ToArray();

                // Check if te type has multiple attributes with same ServiceType.
                var duplicates = GetDuplicates(attributes);

                if (duplicates.Any())
                {
                    throw new InvalidOperationException(
                        $@"Type ""{type.FullName}"" has multiple {nameof(ServiceDescriptorAttribute).Except("Attribute").ToString()} attributes with the same service type.");
                }

                foreach (var attribute in attributes)
                {
                    var serviceTypes = attribute.GetServiceTypes(type);

                    foreach (var serviceType in serviceTypes)
                    {
                        var descriptor = new ServiceDescriptor(serviceType, type, attribute.Lifetime);

                        registrationStrategy.Apply(services, descriptor);
                    }
                }
            }
        }

        private static IEnumerable<ServiceDescriptorAttribute> GetDuplicates(IEnumerable<ServiceDescriptorAttribute> attributes)
        {
            return attributes.GroupBy(s => s.ServiceType).SelectMany(grp => grp.Skip(1));
        }
    }
}

namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ImplementationTypeFilter : IImplementationTypeFilter
    {
        public IEnumerable<Type> Types { get; private set; }

        public ImplementationTypeFilter(IEnumerable<Type> types) => Types = types;

        public IImplementationTypeFilter AssignableTo<T>()
        {
            return AssignableTo(typeof(T));
        }

        public IImplementationTypeFilter AssignableTo(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return AssignableToAny(type);
        }

        public IImplementationTypeFilter AssignableToAny(params Type[] types)
        {
            return AssignableToAny(types.AsEnumerable());
        }

        public IImplementationTypeFilter AssignableToAny(IEnumerable<Type> types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            return Where(t => types.Any(t.IsAssignableTo));
        }

        public IImplementationTypeFilter WithAttribute<T>() where T : Attribute
        {
            return WithAttribute(typeof(T));
        }

        public IImplementationTypeFilter WithAttribute(Type attributeType)
        {
            attributeType = attributeType ?? throw new ArgumentNullException(nameof(attributeType));

            return Where(t => t.HasAttribute(attributeType));
        }

        public IImplementationTypeFilter WithAttribute<T>(Func<T, bool> predicate) where T : Attribute
        {
            predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            return Where(t => t.HasAttribute(predicate));
        }

        public IImplementationTypeFilter WithoutAttribute<T>() where T : Attribute
        {
            return WithoutAttribute(typeof(T));
        }

        public IImplementationTypeFilter WithoutAttribute(Type attributeType)
        {
            attributeType = attributeType ?? throw new ArgumentNullException(nameof(attributeType));

            return Where(t => !t.HasAttribute(attributeType));
        }

        public IImplementationTypeFilter WithoutAttribute<T>(Func<T, bool> predicate) where T : Attribute
        {
            predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            return Where(t => !t.HasAttribute(predicate));
        }

        public IImplementationTypeFilter InNamespaceOf<T>()
        {
            return InNamespaceOf(typeof(T));
        }

        public IImplementationTypeFilter InNamespaceOf(params Type[] types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            return InNamespaces(types.Select(t => t.Namespace));
        }

        public IImplementationTypeFilter InNamespaces(params string[] namespaces)
        {
            return InNamespaces(namespaces.AsEnumerable());
        }

        public IImplementationTypeFilter InNamespaces(IEnumerable<string> namespaces)
        {
            namespaces = namespaces ?? throw new ArgumentNullException(nameof(namespaces));

            return Where(t => namespaces.Any(t.IsInNamespace));
        }

        public IImplementationTypeFilter NotInNamespaceOf<T>()
        {
            return NotInNamespaceOf(typeof(T));
        }

        public IImplementationTypeFilter NotInNamespaceOf(params Type[] types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            return NotInNamespaces(types.Select(t => t.Namespace));
        }

        public IImplementationTypeFilter NotInNamespaces(params string[] namespaces)
        {
            return NotInNamespaces(namespaces.AsEnumerable());
        }

        public IImplementationTypeFilter NotInNamespaces(IEnumerable<string> namespaces)
        {
            namespaces = namespaces ?? throw new ArgumentNullException(nameof(namespaces));

            return Where(t => namespaces.All(ns => !t.IsInNamespace(ns)));
        }

        public IImplementationTypeFilter Where(Func<Type, bool> predicate)
        {
            predicate = predicate ?? throw new NotImplementedException(nameof(predicate));

            Types = Types.Where(predicate);

            return this;
        }
    }
}
﻿namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    internal class ImplementationTypeSelector : TypeSourceSelector, IImplementationTypeSelector, ISelector
    {
        protected ImplementationTypeSelector(IEnumerable<Type> types)
        {
            Types = types;
        }

        protected IEnumerable<Type> Types { get; }

        public IServiceTypeSelector AddClasses()
        {
            return AddClasses(publicOnly: false);
        }

        public IServiceTypeSelector AddClasses(bool publicOnly)
        {
            var classes = GetNonAbstractClasses(publicOnly);

            return AddSelector(classes);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action)
        {
            return AddClasses(action, publicOnly: false);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));

            var classes = GetNonAbstractClasses(publicOnly);

            var filter = new ImplementationTypeFilter(classes);

            action(filter);

            return AddSelector(filter.Types);
        }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            if (Selectors.Count == 0)
            {
                AddClasses();
            }

            foreach (var selector in Selectors)
            {
                selector.Populate(services, registrationStrategy);
            }
        }

        private IServiceTypeSelector AddSelector(IEnumerable<Type> types)
        {
            var selector = new ServiceTypeSelector(types);

            Selectors.Add(selector);

            return selector;
        }

        private IEnumerable<Type> GetNonAbstractClasses(bool publicOnly)
        {
            return Types.Where(t => t.IsNonAbstractClass(publicOnly));

        }
    }
}
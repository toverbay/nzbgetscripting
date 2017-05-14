namespace NzbGetScripting
{
    using System;

    internal interface IImplementationTypeSelector
    {
        IServiceTypeSelector AddClasses();

        IServiceTypeSelector AddClasses(bool publicOnly);

        IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> filter);

        IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> filter, bool publicOnly);
    }
}
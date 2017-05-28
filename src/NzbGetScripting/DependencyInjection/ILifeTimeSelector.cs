namespace NzbGetScripting
{
    using Microsoft.Extensions.DependencyInjection;

    internal interface ILifeTimeSelector
    {
        IImplementationTypeSelector WithSingletonLifetime();

        IImplementationTypeSelector WithScopedLifetime();

        IImplementationTypeSelector WithTransientLifetime();

        IImplementationTypeSelector WithLifetime(ServiceLifetime lifetime);
    }
}
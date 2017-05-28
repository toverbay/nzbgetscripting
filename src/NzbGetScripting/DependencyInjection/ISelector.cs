namespace NzbGetScripting
{
    using Microsoft.Extensions.DependencyInjection;

    internal interface ISelector
    {
        void Populate(IServiceCollection services, RegistrationStrategy registrationStrategy);
    }
}
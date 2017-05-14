namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;

    internal interface ITypeSelector : IFluentInterface
    {
        IServiceTypeSelector AddTypes(params Type[] types);

        IServiceTypeSelector Addtypes(IEnumerable<Type> types);
    }
}
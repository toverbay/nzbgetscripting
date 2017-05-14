namespace NzbGetScripting
{
    using System;

    [Flags]
    public enum ReplacementBehavior
    {
        Default = 0,
        ServiceType = 1,
        ImplementationType = 2,
        All = ServiceType | ImplementationType
    }
}
// NzbGetScripting

namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Converts an enum value into a string token.
    /// </summary>
    public sealed class EnumTokenizer : ObjectTokenizer<Enum>
    {
    }

    /// <summary>
    /// Interim class used by <see cref="EnumTokenizer"/> to enforce 
    /// enum type constraints.
    /// </summary>
    /// <remarks>This class is for internal use only.</remarks>
    /// <typeparam name="TClass">The type</typeparam>
    public abstract class ObjectTokenizer<TClass> where TClass : class
    {
        public static IEnumerable<string> Tokenize<TEnum>(TEnum value)
        where TEnum : struct, TClass
        {
            var intValue = Convert.ToInt32(value);

#pragma warning disable IDE0007 // Need explicit type here to avoid an un-necessary cast
            foreach (int enumValue in Enum.GetValues(typeof(TEnum)))
#pragma warning restore IDE0007 // Use implicit type
            {
                if ((intValue & enumValue) != 0)
                {
                    yield return Enum.GetName(typeof(TEnum), enumValue).
                        SplitPascalCase("-");
                }
            }
        }
    }
}

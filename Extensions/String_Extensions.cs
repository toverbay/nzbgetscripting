namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    static class String_Extensions
    {
        private static readonly Regex
            rxPascalCaseSplitter = new Regex(
                @"(?<!^)([A-Z][a-z]|(?<=[a-z])[^a-z]|(?<=[A-Z])[0-9_])",
                RegexOptions.Compiled);

        public static IEnumerable<string> WrapOnWordBoundary(this string sentence, int maxLineLength)
        {
            // Split on any whitespace characters
            var words = sentence.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length < 1)
            {
                yield break;
            }

            var newLine = new StringBuilder();

            foreach (var word in words)
            {
                if ((newLine.Length + word.Length) > maxLineLength)
                {
                    yield return newLine.ToString().Trim();
                    newLine.Clear();
                }

                newLine.AppendFormat("{0} ", word);
            }

            if (newLine.Length > 0)
            {
                yield return newLine.ToString().Trim();
            }
        }

        public static string SplitPascalCase(this string value, string delimiter = " ")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return rxPascalCaseSplitter.Replace(value, $"{delimiter}$1");
        }
    }
}

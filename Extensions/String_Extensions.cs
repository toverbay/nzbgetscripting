namespace NzbGetScripting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    static class String_Extensions
    {
        public const string NullTextToken = "[null]";

        private static readonly Regex
            rxPascalCaseSplitter = new Regex(
                @"(?<!^)([A-Z][a-z]|(?<=[a-z])[^a-z]|(?<=[A-Z])[0-9_])",
                RegexOptions.Compiled);

        private static readonly char[] WhiteSpaceChars = new char[]
        {
            ' ', '\t', '\r', '\n', '\f', '\v',
        };

        public static string ToStringOrDefault(this string value, string defaultText = NullTextToken)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultText;
            }
            else
            {
                return value;
            }
        }

        public static string ToStringOrDefault(this object value, string defaultText = NullTextToken)
        {
            if (!ReferenceEquals(null, value))
            {
                return value.ToString();
            }
            else
            {
                return defaultText;
            }
        }

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

        public static string ToQuotedString(this object value, string quoteChar = "'", string defaultValue = NullTextToken)
        {
            return ToQuotedString(value, quoteChar, quoteChar, defaultValue);
        }

        public static string ToQuotedString(this object value, string startQuoteChar, string endQuoteChar, string defaultValue = NullTextToken)
        {
            var strValue = ToStringOrDefault(value, string.Empty);

            if (string.IsNullOrWhiteSpace(strValue))
            {
                return defaultValue;
            }
            else
            {
                return startQuoteChar + value + endQuoteChar;
            }
        }

        public static string ToUnquotedString(this string value, string quoteChar = "\"", string defaultValue = NullTextToken)
        {
            return ToUnquotedString(value, quoteChar, quoteChar, defaultValue);
        }

        public static string ToUnquotedString(this string value, string beginQuoteChar, string endQuoteChar, string defaultValue = NullTextToken)
        {
            var strValue = ToStringOrDefault(value, string.Empty);

            if (string.IsNullOrWhiteSpace(strValue))
            {
                return defaultValue;
            }
            else
            {
                return strValue
                    .TrimStart(WhiteSpaceChars.Concat(beginQuoteChar).ToArray())
                    .TrimEnd(WhiteSpaceChars.Concat(endQuoteChar).ToArray());
            }
        }
    }
}

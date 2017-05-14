using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace NzbGetScripting
{
    [Flags]
    public enum NzbGetScriptType
    {
        None = 0,
        PostProcessing = 1,
        Scan = 2,
        Queue = 4,
        Scheduler = 8
    }

    public static class NzbGetContext
    {
        public static string GenerateScriptTypeToken(NzbGetScriptType scriptType)
        {
            return string.Join("/", EnumTokenizer.Tokenize(scriptType).ToArray())
                .ToUpperInvariant();
            
        }

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static string GenerateScriptHeader(NzbGetScriptType scriptType, string shortDescription, string longDescription)
        {
            var scriptTypeToken = GenerateScriptTypeToken(scriptType);
            var lineLength = scriptTypeToken.Length + 29;

            var sb = new StringBuilder();

            sb.AppendLine(new string('#', lineLength));
            sb.AppendLine($"### NZBGET {scriptTypeToken} SCRIPT {new string(' ', lineLength - scriptTypeToken.Length - 22)}###");
            sb.AppendLine();
            sb.AppendLine($"### {(string.IsNullOrWhiteSpace(shortDescription) ? "Short description of your script goes here." : shortDescription)}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}

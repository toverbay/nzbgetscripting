using System;
using System.Collections.Generic;
using System.Text;

namespace NzbGetScripting
{
    public class MyScript : NzbScriptBase
    {
        public override NzbGetScriptType Type => NzbGetScriptType.PostProcessing;
        public override string Name => "TheScript";
        public override string ShortDescription => $"This is a script defined right in the {nameof(NzbGetScripting)} assembly";
    }
}

namespace MyCoolScript
{
    using NzbGetScripting;
    using System;
    using System.Collections.Generic;

    public class MyCoolScript : NzbScriptBase
    {
        public MyCoolScript()
        {
        }

        public override NzbGetScriptType Type => NzbGetScriptType.PostProcessing;

        public override string ShortDescription => $"This is a script defined in the external {nameof(MyCoolScript)} assembly";

        public override int Run(IEnumerable<string> args)
        {
            Console.WriteLine($"Running {Name} with args: {string.Join(", ", args)}");

            return Success();
        }
    }
}

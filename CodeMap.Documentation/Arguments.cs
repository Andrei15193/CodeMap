using System.Collections.Generic;
using System.Reflection;

namespace CodeMap.Documentation
{
    public class Arguments
    {
        public static Arguments GetFrom(IEnumerable<string> args)
        {
            var result = new Arguments();
            string name = null;
            foreach (var arg in args)
                if (arg.StartsWith('-'))
                    name = arg.Substring(1);
                else if (name is object)
                {
                    var property = typeof(Arguments).GetRuntimeProperty(name);
                    if (property is object)
                        property.SetValue(result, arg);
                    name = null;
                }

            return result;
        }

        public string OutputPath { get; set; }
    }
}
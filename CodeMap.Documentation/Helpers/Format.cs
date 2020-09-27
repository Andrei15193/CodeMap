using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CodeMap.Documentation.Helpers
{
    public class Format : IHandlebarsHelper
    {
        public string Name
            => nameof(Format);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var formattable = parameters.ElementAtOrDefault(0) as IFormattable ?? throw new ArgumentException("Expected a " + nameof(IFormattable) + " as the first parameter.");
            var format = parameters.ElementAtOrDefault(1) as string ?? throw new ArgumentException("Expected a format string as the second parameter.");
            writer.Write(formattable.ToString(format, CultureInfo.InvariantCulture));
        }
    }
}
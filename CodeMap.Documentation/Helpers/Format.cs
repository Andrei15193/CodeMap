using System;
using System.Globalization;
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class Format : IHandlebarsHelper
    {
        public string Name
            => nameof(Format);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            var formattable = (IFormattable)parameters[0];
            var format = (string)parameters[1];
            writer.Write(formattable.ToString(format, CultureInfo.InvariantCulture));
        }
    }
}
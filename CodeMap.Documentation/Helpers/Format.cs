using System;
using System.Globalization;
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class Format : HandlebarsContextualHelper<IFormattable, string>
    {
        public override string Name
            => nameof(Format);

        public override void Apply(TextWriter writer, PageContext context, IFormattable formattable, string format)
            => writer.Write(formattable.ToString(format, CultureInfo.InvariantCulture));
    }
}
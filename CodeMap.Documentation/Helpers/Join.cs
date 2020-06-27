using HandlebarsDotNet;
using System.Collections;
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class Join : IHandlebarsBlockHelper
    {
        public string Name
            => nameof(Join);

        public void Apply(TextWriter writer, HelperOptions options, dynamic context, params object[] parameters)
        {
            var separator = (string)parameters[0];
            var items = (IEnumerable)parameters[1];
            var isFirst = true;
            foreach (var item in items)
            {
                if (isFirst)
                    isFirst = false;
                else
                    writer.Write(separator);
                options.Template(writer, item);
            }
        }
    }
}
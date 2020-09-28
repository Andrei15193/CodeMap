using System.Collections;
using System.IO;
using System.Linq;

namespace CodeMap.Handlebars.Helpers
{
    public class IsCollection : IHandlebarsHelper
    {
        public string Name
            => nameof(IsCollection);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            if (parameters.FirstOrDefault() is IEnumerable)
                writer.Write(true);
        }
    }
}
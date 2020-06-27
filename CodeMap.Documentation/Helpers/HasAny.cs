using System.Collections;
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class HasAny : IHandlebarsHelper
    {
        public string Name
            => nameof(HasAny);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            var enumerator = ((IEnumerable)parameters[0]).GetEnumerator();
            if (enumerator.MoveNext())
                writer.Write(true);
        }
    }
}
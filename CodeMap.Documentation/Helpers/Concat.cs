using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class Concat : IHandlebarsHelper
    {
        public string Name
            => nameof(Concat);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            foreach (var parameter in parameters)
                writer.Write(parameter);
        }
    }
}
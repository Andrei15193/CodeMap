using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class TypeReferenceLink : IHandlebarsHelper
    {
        public string Name
            => nameof(TypeReferenceLink);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
            => HandlebarsExtensions.WriteTemplate(writer, Name, parameters[0]);
    }
}
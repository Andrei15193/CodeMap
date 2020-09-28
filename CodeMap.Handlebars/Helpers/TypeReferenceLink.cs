using System.IO;
using System.Linq;

namespace CodeMap.Handlebars.Helpers
{
    public class TypeReferenceLink : IHandlebarsHelper
    {
        private readonly TemplateWriter _templateWriter;

        public TypeReferenceLink(TemplateWriter templateWriter)
            => _templateWriter = templateWriter;

        public string Name
            => nameof(TypeReferenceLink);

        public void Apply(TextWriter writer, object context, params object[] parameters)
            => _templateWriter.Write(writer, Name, parameters.ElementAtOrDefault(0));
    }
}
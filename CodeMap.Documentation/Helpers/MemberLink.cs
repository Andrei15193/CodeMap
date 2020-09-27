using System.IO;
using System.Linq;

namespace CodeMap.Documentation.Helpers
{
    public class MemberLink : IHandlebarsHelper
    {
        private readonly TemplateWriter _templateWriter;

        public MemberLink(TemplateWriter templateWriter)
            => _templateWriter = templateWriter;

        public string Name
            => nameof(MemberLink);

        public void Apply(TextWriter writer, object context, params object[] parameters)
            => _templateWriter.Write(writer, Name, parameters.DefaultIfEmpty(context).First());
    }
}
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class MemberLink : IHandlebarsHelper
    {
        public string Name
            => nameof(MemberLink);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
            => HandlebarsExtensions.WriteTemplate(writer, Name, parameters[0]);
    }
}
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class RelatedMembersList : IHandlebarsHelper
    {
        public string Name
            => nameof(RelatedMembersList);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
            => HandlebarsExtensions.WriteTemplate(writer, Name, parameters[0]);
    }
}
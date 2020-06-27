using System.Collections;
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class MemberDefinitionsList : IHandlebarsHelper
    {
        public string Name
            => nameof(MemberDefinitionsList);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
            => HandlebarsExtensions.WriteTemplate(
                writer,
                Name,
                new
                {
                    Title = (string)parameters[0],
                    Definitions = (IEnumerable)parameters[1]
                }
            );
    }
}
using System;
using System.Collections;
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class MemberDefinitionsList : HandlebarsContextualHelper<string, IEnumerable, string>
    {
        public override string Name
            => nameof(MemberDefinitionsList);

        public override void Apply(TextWriter writer, PageContext context, string title, IEnumerable definitions, string includeAccessor)
            => HandlebarsExtensions.WriteTemplate(
                writer,
                Name,
                context.WithData(
                    new
                    {
                        Title = title,
                        Definitions = definitions,
                        IncludeAccessor = string.Equals(includeAccessor, "includeAccessor", StringComparison.OrdinalIgnoreCase)
                    }
                )
            );
    }
}
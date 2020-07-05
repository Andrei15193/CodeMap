using System;
using System.Collections.Generic;
using System.IO;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class MemberDeclarationsList : HandlebarsContextualHelper<string, IEnumerable<DeclarationNode>, string>
    {
        public override string Name
            => nameof(MemberDeclarationsList);

        public override void Apply(TextWriter writer, PageContext context, string title, IEnumerable<DeclarationNode> declarations, string includeAccessor)
            => HandlebarsExtensions.WriteTemplate(
                writer,
                Name,
                context.WithData(
                    new
                    {
                        Title = title,
                        Definitions = declarations,
                        IncludeAccessor = string.Equals(includeAccessor, "includeAccessor", StringComparison.OrdinalIgnoreCase)
                    }
                )
            );
    }
}
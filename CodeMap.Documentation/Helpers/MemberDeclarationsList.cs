using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class MemberDeclarationsList : IHandlebarsHelper
    {
        private readonly TemplateWriter _templateWriter;

        public MemberDeclarationsList(TemplateWriter templateWriter)
            => _templateWriter = templateWriter;

        public string Name
            => nameof(MemberDeclarationsList);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var title = parameters.ElementAtOrDefault(0) as string ?? throw new ArgumentException("Expected title parameter.");
            var declarations = parameters.ElementAtOrDefault(1) as IEnumerable<DeclarationNode> ?? throw new ArgumentException("Expected declarations parameter.");
            var includeAccessor = parameters.ElementAtOrDefault(2) as string;

            _templateWriter.Write(
                writer,
                Name,
                new
                {
                    Title = title,
                    Definitions = declarations,
                    IncludeAccessor = string.Equals(includeAccessor, "includeAccessor", StringComparison.OrdinalIgnoreCase)
                }
            );
        }
    }
}
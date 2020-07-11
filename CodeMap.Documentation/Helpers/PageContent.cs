using System.IO;
using System.Text.RegularExpressions;

namespace CodeMap.Documentation.Helpers
{
    public class PageContent : HandlebarsContextualHelper
    {
        public override string Name
            => nameof(PageContent);

        public override void Apply(TextWriter writer, PageContext context)
        {
            var declarationNodeType = Regex.Match(context.DeclarationNode.GetType().Name, @"^[A-Z][a-z]+").Value;
            writer.WriteTemplate(declarationNodeType, context);
        }
    }
}
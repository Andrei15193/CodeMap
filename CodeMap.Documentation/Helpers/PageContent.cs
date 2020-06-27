using System.IO;
using System.Text.RegularExpressions;

namespace CodeMap.Documentation.Helpers
{
    public class PageContent : IHandlebarsHelper
    {
        public string Name
            => nameof(PageContent);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            var pageContext = (PageContext)context;
            var declarationNodeType = Regex.Match(pageContext.DeclarationNode.GetType().Name, @"^[A-Z][a-z]+").Value;
            writer.WriteTemplate(declarationNodeType, pageContext);
        }
    }
}
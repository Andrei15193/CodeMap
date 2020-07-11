using System.Collections.Generic;
using System.IO;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.Helpers
{
    public class DocumentationContent : HandlebarsContextualHelper<MultiParameter<DocumentationElement, IEnumerable<DocumentationElement>>>
    {
        public override string Name
            => nameof(DocumentationContent);

        public override void Apply(TextWriter writer, PageContext context, MultiParameter<DocumentationElement, IEnumerable<DocumentationElement>> parameter)
        {
            var visitor = new HtmlWriterDocumentationVisitor(writer, context);
            parameter
                .Handle<DocumentationElement>(documentationElement => documentationElement.Accept(visitor))
                .Handle<IEnumerable<DocumentationElement>>(documentationElements =>
                {
                    foreach (var documentationElement in documentationElements)
                        documentationElement.Accept(visitor);
                });
        }
    }
}
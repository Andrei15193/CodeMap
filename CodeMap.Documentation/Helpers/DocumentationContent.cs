using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class DocumentationContent : IHandlebarsHelper
    {
        public string Name
            => nameof(DocumentationContent);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            var pageContext = context is TemplateContext templateContext
                ? templateContext.PageContext
                : context is DeclarationNode declarationNode
                ? new PageContext(declarationNode)
                : (PageContext)context;
            var visitor = new HtmlWriterDocumentationVisitor(writer, pageContext);
            switch (parameters[0])
            {
                case DocumentationElement documentationElement:
                    documentationElement.Accept(visitor);
                    break;

                case IEnumerable<DocumentationElement> documentationElements:
                    foreach (var documentationElement in documentationElements)
                        documentationElement.Accept(visitor);
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameters[0].GetType().Name}'");
            }
        }
    }
}
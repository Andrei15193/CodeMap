using System;
using System.Collections.Generic;
using System.IO;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.Helpers
{
    public class DocumentationContent : HandlebarsContextualHelper<object>
    {
        public override string Name
            => nameof(DocumentationContent);

        public override void Apply(TextWriter writer, PageContext context, object parameter)
        {
            var visitor = new HtmlWriterDocumentationVisitor(writer, context);
            switch (parameter)
            {
                case DocumentationElement documentationElement:
                    documentationElement.Accept(visitor);
                    break;

                case IEnumerable<DocumentationElement> documentationElements:
                    foreach (var documentationElement in documentationElements)
                        documentationElement.Accept(visitor);
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'");
            }
        }
    }
}
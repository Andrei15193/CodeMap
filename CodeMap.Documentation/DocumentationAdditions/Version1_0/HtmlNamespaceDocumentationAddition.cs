using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using System;

namespace CodeMap.Documentation.DocumentationAdditions.Version1_0
{
    internal class HtmlNamespaceDocumentationAddition : NamespaceDocumentationAddition
    {
        public override bool CanApply(NamespaceDeclaration @namespace)
            => string.Equals("CodeMap.Html", @namespace.Name, StringComparison.OrdinalIgnoreCase);

        public override SummaryDocumentationElement GetSummary(NamespaceDeclaration @namespace)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text("Contains types for generating HTML pages.")
                )
            );
    }
}
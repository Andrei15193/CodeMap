using System;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.DocumentationAdditions.Version1_0
{
    public class CodeMapHandlebarsNamespaceDocumentationAddition : NamespaceDocumentationAddition
    {
        public override bool CanApply(NamespaceDeclaration @namespace)
            => string.Equals("CodeMap.Handlebars", @namespace.Name, StringComparison.OrdinalIgnoreCase);

        public override SummaryDocumentationElement GetSummary(NamespaceDeclaration @namespace)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text("Contains the "),
                    DocumentationElement.Hyperlink("https://github.com/Handlebars-Net/Handlebars.Net", DocumentationElement.Text("Handlebars.NET")),
                    DocumentationElement.Text(" based type declarations and implementation for generating documentation.")
                )
            );
    }
}
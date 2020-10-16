using System;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.DocumentationAdditions.Version1_0
{
    public class CodeMapHandlebarsHelpersNamespaceDocumentationAddition : NamespaceDocumentationAddition
    {
        public override bool CanApply(NamespaceDeclaration @namespace)
            => string.Equals("CodeMap.Handlebars.Helpers", @namespace.Name, StringComparison.OrdinalIgnoreCase);

        public override SummaryDocumentationElement GetSummary(NamespaceDeclaration @namespace)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text("Contains the "),
                    DocumentationElement.Hyperlink("https://github.com/Handlebars-Net/Handlebars.Net", "Handlebars.NET"),
                    DocumentationElement.Text(" helpers that are used in the default Handlebars templates.")
                )
            );
    }
}
using System;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.Additions._1_0
{
    public class DeclarationNodesNamespaceDocumentationAddition : NamespaceDocumentationAddition
    {
        public override bool CanApply(NamespaceDeclaration @namespace)
            => string.Equals("CodeMap.DeclarationNodes", @namespace.Name, StringComparison.OrdinalIgnoreCase);

        public override SummaryDocumentationElement GetSummary(NamespaceDeclaration @namespace)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text("Contains the declaration node definitions corresponding to declarations of assembly members (classes, properties, methods and so on). This is the entry point for generating documentation.")
                )
            );
    }
}
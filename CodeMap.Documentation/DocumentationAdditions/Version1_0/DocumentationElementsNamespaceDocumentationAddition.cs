using System;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.DocumentationAdditions.Version1_0
{
    internal class DocumentationElementsNamespaceDocumentationAddition : NamespaceDocumentationAddition
    {
        public override bool CanApply(NamespaceDeclaration @namespace)
            => string.Equals("CodeMap.DocumentationElements", @namespace.Name, StringComparison.OrdinalIgnoreCase);

        public override SummaryDocumentationElement GetSummary(NamespaceDeclaration @namespace)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text("Contains the documentation element definitions corresponding to XML elements that are extracted from a documentation XML file associated to an assembly.")
                )
            );
    }
}
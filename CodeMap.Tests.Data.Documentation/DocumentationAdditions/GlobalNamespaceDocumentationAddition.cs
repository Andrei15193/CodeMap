using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Tests.Data.Documentation.DocumentationAdditions
{
    public class GlobalNamespaceDocumentationAddition : NamespaceDocumentationAddition
    {
        public override bool CanApply(NamespaceDeclaration namespaceDeclaration)
            => namespaceDeclaration is GlobalNamespaceDeclaration;

        public override SummaryDocumentationElement GetSummary(NamespaceDeclaration namespaceDeclaration)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text("This is the global namespace. These are not generally used as it can introduce name resolution conflicts that cannot be resolved by referencing through a namespace.")
                )
            );
    }
}
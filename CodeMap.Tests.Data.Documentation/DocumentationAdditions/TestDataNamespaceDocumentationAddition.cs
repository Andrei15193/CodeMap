using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Tests.Data.Documentation.DocumentationAdditions
{
    public class TestDataNamespaceDocumentationAddition : NamespaceDocumentationAddition
    {
        public override bool CanApply(NamespaceDeclaration namespaceDeclaration)
            => !(namespaceDeclaration is GlobalNamespaceDeclaration);

        public override SummaryDocumentationElement GetSummary(NamespaceDeclaration namespaceDeclaration)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text("This namespace contains all kinds of declarations, the purpose is to cover as many cases as possible to ensure the related documentation elements are generated property.")
                )
            );
    }
}
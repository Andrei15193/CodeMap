using System.Collections.Generic;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Tests.Data.Documentation.DocumentationAdditions
{
    public class TestDataAssemblyDocumentationAddition : AssemblyDocumentationAddition
    {
        public override bool CanApply(AssemblyDeclaration assemblyDeclaration)
            => true;

        public override SummaryDocumentationElement GetSummary(AssemblyDeclaration assemblyDeclaration)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text("This site is generated from the test data assembly and it includes all members (public and private).")
                )
            );

        public override IEnumerable<NamespaceDocumentationAddition> GetNamespaceAdditions(AssemblyDeclaration assemblyDeclaration)
        {
            yield return new GlobalNamespaceDocumentationAddition();
            yield return new TestDataNamespaceDocumentationAddition();
        }
    }
}
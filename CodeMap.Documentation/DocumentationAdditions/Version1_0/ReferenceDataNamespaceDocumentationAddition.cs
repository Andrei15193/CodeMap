using System;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.DocumentationAdditions.Version1_0
{
    public class ReferenceDataNamespaceDocumentationAddition : NamespaceDocumentationAddition
    {
        public override bool CanApply(NamespaceDeclaration @namespace)
            => string.Equals("CodeMap.ReferenceData", @namespace.Name, StringComparison.OrdinalIgnoreCase);

        public override SummaryDocumentationElement GetSummary(NamespaceDeclaration @namespace)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text("Contains member reference definitions that can be used to create hyperlinks to referred members defined both in the documented library and dependent assemblies.")
                )
            );
    }
}
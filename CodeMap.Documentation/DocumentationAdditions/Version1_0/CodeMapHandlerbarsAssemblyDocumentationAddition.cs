using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.Handlebars;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.DocumentationAdditions.Version1_0
{
    public class CodeMapHandlerbarsAssemblyDocumentationAddition : AssemblyDocumentationAddition
    {
        public override bool CanApply(AssemblyDeclaration assemblyDeclaration)
            => assemblyDeclaration.Version.Major == 1 && assemblyDeclaration.Version.Minor == 0;

        public override SummaryDocumentationElement GetSummary(AssemblyDeclaration assemblyDeclaration)
            => DocumentationElement.Summary(
                DocumentationElement.Paragraph(
                    DocumentationElement.Text(
                        assemblyDeclaration
                            .Attributes
                            .Single(attribute => attribute.Type == typeof(AssemblyDescriptionAttribute))
                            .PositionalParameters
                            .Single()
                            .Value
                            .ToString()
                    )
                )
            );

        public override IEnumerable<MemberReferenceDocumentationElement> GetRelatedMembers(AssemblyDeclaration assembly)
        {
            var memberReferenceFactory = new MemberReferenceFactory();
            yield return DocumentationElement.MemberReference(memberReferenceFactory.Create(typeof(HandlebarsTemplateWriter)));
        }

        public override IEnumerable<NamespaceDocumentationAddition> GetNamespaceAdditions(AssemblyDeclaration assembly)
        {
            yield return new CodeMapHandlebarsNamespaceDocumentationAddition();
            yield return new CodeMapHandlebarsHelpersNamespaceDocumentationAddition();
        }
    }
}
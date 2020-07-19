using System.Collections.Generic;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Tests.DeclarationNodes.Mocks
{
    public class NamespaceDocumentationAdditionMock : NamespaceDocumentationAddition
    {
        public SummaryDocumentationElement Summary { get; set; }

        public RemarksDocumentationElement Remarks { get; set; }

        public IEnumerable<ExampleDocumentationElement> Examples { get; set; }

        public IEnumerable<MemberReferenceDocumentationElement> RelatedMembers { get; set; }

        public override bool CanApply(NamespaceDeclaration namespaceDeclaration)
            => true;

        public override SummaryDocumentationElement GetSummary(NamespaceDeclaration namespaceDeclaration)
            => Summary;

        public override RemarksDocumentationElement GetRemarks(NamespaceDeclaration namespaceDeclaration)
            => Remarks;

        public override IEnumerable<ExampleDocumentationElement> GetExamples(NamespaceDeclaration namespaceDeclaration)
            => Examples;

        public override IEnumerable<MemberReferenceDocumentationElement> GetRelatedMembers(NamespaceDeclaration namespaceDeclaration)
            => RelatedMembers;
    }
}
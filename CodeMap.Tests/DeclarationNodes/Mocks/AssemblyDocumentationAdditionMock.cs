using System.Collections.Generic;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Tests.DeclarationNodes.Mocks
{
    public class AssemblyDocumentationAdditionMock : AssemblyDocumentationAddition
    {
        public bool Skip { get; set; }

        public SummaryDocumentationElement Summary { get; set; }

        public RemarksDocumentationElement Remarks { get; set; }

        public IEnumerable<ExampleDocumentationElement> Examples { get; set; }

        public IEnumerable<MemberReferenceDocumentationElement> RelatedMembers { get; set; }

        public IEnumerable<NamespaceDocumentationAddition> NamespaceAdditions { get; set; }

        public override bool CanApply(AssemblyDeclaration assemblyDeclaration)
            => !Skip;

        public override SummaryDocumentationElement GetSummary(AssemblyDeclaration assemblyDeclaration)
            => Summary;

        public override RemarksDocumentationElement GetRemarks(AssemblyDeclaration assemblyDeclaration)
            => Remarks;

        public override IEnumerable<ExampleDocumentationElement> GetExamples(AssemblyDeclaration assemblyDeclaration)
            => Examples;

        public override IEnumerable<MemberReferenceDocumentationElement> GetRelatedMembers(AssemblyDeclaration assemblyDeclaration)
            => RelatedMembers;

        public override IEnumerable<NamespaceDocumentationAddition> GetNamespaceAdditions(AssemblyDeclaration assemblyDeclaration)
            => NamespaceAdditions;
    }
}
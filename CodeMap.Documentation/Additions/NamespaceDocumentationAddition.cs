using System.Collections.Generic;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.Additions
{
    public class NamespaceDocumentationAddition
    {
        public SummaryDocumentationElement Summary { get; set; }

        public RemarksDocumentationElement Remarks { get; set; }

        public IReadOnlyList<ExampleDocumentationElement> Examples { get; set; }

        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; set; }

        public IReadOnlyDictionary<string, NamespaceDocumentationAddition> NamespaceAdditions { get; set; }
    }
}
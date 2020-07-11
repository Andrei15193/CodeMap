using System.Collections.Generic;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.Additions
{
    public abstract class NamespaceDocumentationAddition
    {
        protected NamespaceDocumentationAddition()
        {
        }

        public abstract bool CanApply(NamespaceDeclaration @namespace);

        public virtual SummaryDocumentationElement GetSummary(NamespaceDeclaration @namespace)
            => null;

        public virtual RemarksDocumentationElement GetRemarks(NamespaceDeclaration @namespace)
            => null;

        public virtual IEnumerable<ExampleDocumentationElement> GetExamples(NamespaceDeclaration @namespace)
            => null;

        public virtual IEnumerable<MemberReferenceDocumentationElement> GetRelatedMembers(NamespaceDeclaration @namespace)
            => null;
    }
}
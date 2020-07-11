using System.Collections.Generic;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.Additions
{
    public abstract class AssemblyDocumentationAddition
    {
        protected AssemblyDocumentationAddition()
        {
        }

        public abstract bool CanApply(AssemblyDeclaration assembly);

        public virtual SummaryDocumentationElement GetSummary(AssemblyDeclaration assembly)
            => null;

        public virtual RemarksDocumentationElement GetRemarks(AssemblyDeclaration assembly)
            => null;

        public virtual IEnumerable<ExampleDocumentationElement> GetExamples(AssemblyDeclaration assembly)
            => null;

        public virtual IEnumerable<MemberReferenceDocumentationElement> GetRelatedMembers(AssemblyDeclaration assembly)
            => null;

        public virtual IEnumerable<NamespaceDocumentationAddition> GetNamespaceAdditions(AssemblyDeclaration assemblyDeclaration)
            => null;
    }
}
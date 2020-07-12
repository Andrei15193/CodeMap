using System.Collections.Generic;
using CodeMap.DocumentationElements;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documentation addition at the namespace level.</summary>
    public abstract class NamespaceDocumentationAddition
    {
        /// <summary>Initializes a new instance of the <see cref="NamespaceDocumentationAddition"/> class.</summary>
        protected NamespaceDocumentationAddition()
        {
        }

        /// <summary>
        /// A filtering predicate that indicates whether the current instance can be applied to the provided <paramref name="namespaceDeclaration"/>.
        /// </summary>
        /// <param name="namespaceDeclaration">The <see cref="NamespaceDeclaration"/> to check.</param>
        /// <returns>Returns <c>true</c> if the current addition is applicable; <c>false</c> otherwise.</returns>
        public abstract bool CanApply(NamespaceDeclaration namespaceDeclaration);

        /// <summary>Gets the summary addition for the provided <paramref name="namespaceDeclaration"/>.</summary>
        /// <param name="namespaceDeclaration">The <see cref="NamespaceDeclaration"/> for which to get the summary addition.</param>
        /// <returns>Returns a <see cref="SummaryDocumentationElement"/> for the provided <paramref name="namespaceDeclaration"/>.</returns>
        public virtual SummaryDocumentationElement GetSummary(NamespaceDeclaration namespaceDeclaration)
            => null;

        /// <summary>Gets the remarks addition for the provided <paramref name="namespaceDeclaration"/>.</summary>
        /// <param name="namespaceDeclaration">The <see cref="NamespaceDeclaration"/> for which to get the remarks addition.</param>
        /// <returns>Returns a <see cref="RemarksDocumentationElement"/> for the provided <paramref name="namespaceDeclaration"/>.</returns>
        public virtual RemarksDocumentationElement GetRemarks(NamespaceDeclaration namespaceDeclaration)
            => null;

        /// <summary>Gets the example additions for the provided <paramref name="namespaceDeclaration"/>.</summary>
        /// <param name="namespaceDeclaration">The <see cref="NamespaceDeclaration"/> for which to get the example additions.</param>
        /// <returns>Returns a collection of <see cref="ExampleDocumentationElement"/> for the provided <paramref name="namespaceDeclaration"/>.</returns>
        public virtual IEnumerable<ExampleDocumentationElement> GetExamples(NamespaceDeclaration namespaceDeclaration)
            => null;

        /// <summary>Gets the related members addition for the provided <paramref name="namespaceDeclaration"/>.</summary>
        /// <param name="namespaceDeclaration">The <see cref="NamespaceDeclaration"/> for which to get the related members addition.</param>
        /// <returns>Returns a collection of <see cref="MemberReferenceDocumentationElement"/> for the provided <paramref name="namespaceDeclaration"/>.</returns>
        public virtual IEnumerable<MemberReferenceDocumentationElement> GetRelatedMembers(NamespaceDeclaration namespaceDeclaration)
            => null;
    }
}
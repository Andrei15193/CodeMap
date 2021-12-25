using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented namespace.</summary>
    public class NamespaceDeclaration : DeclarationNode
    {
        internal NamespaceDeclaration(NamespaceReference namespaceReference)
            : base(namespaceReference)
        {
        }

        /// <summary>The namespace name.</summary>
        public string Name { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyDeclaration Assembly { get; internal set; }

        /// <summary>The declared types in this namespace.</summary>
        public IReadOnlyCollection<TypeDeclaration> DeclaredTypes { get; internal set; }

        /// <summary>The declared enums in this namespace.</summary>
        public IReadOnlyCollection<EnumDeclaration> Enums { get; internal set; }

        /// <summary>The declared delegates in this namespace.</summary>
        public IReadOnlyCollection<DelegateDeclaration> Delegates { get; internal set; }

        /// <summary>The declared interfaces in this namespace.</summary>
        public IReadOnlyCollection<InterfaceDeclaration> Interfaces { get; internal set; }

        /// <summary>The declared classes in this namespace.</summary>
        public IReadOnlyCollection<ClassDeclaration> Classes { get; internal set; }

        /// <summary>The declared  structs in this namespace.</summary>
        public IReadOnlyCollection<StructDeclaration> Structs { get; internal set; }

        /// <summary>The declared records in this namespace.</summary>
        public IReadOnlyCollection<RecordDeclaration> Records { get; internal set; }

        /// <summary>The namespace summary.</summary>
        public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The namespace remarks.</summary>
        public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The namespace examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The namespace related members.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitNamespace(this);

        /// <summary>Applies the first applicable <see cref="NamespaceDocumentationAddition"/> from the provided <paramref name="additions"/>.</summary>
        /// <param name="additions">The <see cref="NamespaceDocumentationAddition"/>s to look through.</param>
        /// <returns>Returns the current <see cref="NamespaceDeclaration"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="additions"/> is <c>null</c>.</exception>
        /// <remarks>
        /// It is possible to have multiple <see cref="NamespaceDocumentationAddition"/>s when working with large libraries. There may
        /// be one <see cref="NamespaceDocumentationAddition"/> for each major version. By providing them as a list it is easier to
        /// maintain because each <see cref="NamespaceDocumentationAddition"/> has its own predicate which in turn will determine which
        /// addition should be used specifically for the current <see cref="NamespaceDeclaration"/>.
        /// </remarks>
        public NamespaceDeclaration Apply(IEnumerable<NamespaceDocumentationAddition> additions)
        {
            if (additions is null)
                throw new ArgumentNullException(nameof(additions));

            var addition = additions.FirstOrDefault(addition => addition.CanApply(this));
            if (addition is object)
            {
                Summary = addition.GetSummary(this) ?? Summary;
                Remarks = addition.GetRemarks(this) ?? Remarks;
                Examples = addition.GetExamples(this).ToReadOnlyList() ?? Examples;
                RelatedMembers = addition.GetRelatedMembers(this).ToReadOnlyList() ?? RelatedMembers;
            }

            return this;
        }

        /// <summary>Applies the first applicable <see cref="NamespaceDocumentationAddition"/> from the provided <paramref name="additions"/>.</summary>
        /// <param name="additions">The <see cref="NamespaceDocumentationAddition"/>s to look through.</param>
        /// <returns>Returns the current <see cref="NamespaceDeclaration"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="additions"/> is <c>null</c>.</exception>
        /// <remarks>
        /// It is possible to have multiple <see cref="NamespaceDocumentationAddition"/>s when working with large libraries. There may
        /// be one <see cref="NamespaceDocumentationAddition"/> for each major version. By providing them as a list it is easier to
        /// maintain because each <see cref="NamespaceDocumentationAddition"/> has its own predicate which in turn will determine which
        /// addition should be used specifically for the current <see cref="NamespaceDeclaration"/>.
        /// </remarks>
        public NamespaceDeclaration Apply(params NamespaceDocumentationAddition[] additions)
            => Apply((IEnumerable<NamespaceDocumentationAddition>)additions);
    }
}
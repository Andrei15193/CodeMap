using CodeMap.DocumentationElements;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented namespace.</summary>
    public class NamespaceDeclaration : DeclarationNode
    {
        internal NamespaceDeclaration()
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

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DeclarationNodeVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitNamespaceAsync(this, cancellationToken);
    }
}
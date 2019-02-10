using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented namespace.</summary>
    public class NamespaceDocumentationElement : DocumentationElement
    {
        internal NamespaceDocumentationElement()
        {
        }

        /// <summary>The namespace name.</summary>
        public string Name { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyDocumentationElement Assembly { get; internal set; }

        /// <summary>The declared enums in this namespace.</summary>
        public IReadOnlyCollection<EnumDocumentationElement> Enums { get; internal set; }

        /// <summary>The declared delegates in this namespace.</summary>
        public IReadOnlyCollection<DelegateDocumentationElement> Delegates { get; internal set; }

        /// <summary>The declared interfaces in this namespace.</summary>
        public IReadOnlyCollection<InterfaceDocumentationElement> Interfaces { get; internal set; }

        /// <summary>The declared classes in this namespace.</summary>
        public IReadOnlyCollection<ClassDocumentationElement> Classes { get; internal set; }

        /// <summary>The declared  structs in this namespace.</summary>
        public IReadOnlyCollection<StructDocumentationElement> Structs { get; internal set; }

        /// <summary>The namespace summary.</summary>
        new public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The namespace remarks.</summary>
        new public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The namespace examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The namespace related members.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            throw new NotImplementedException();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
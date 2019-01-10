using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents an external (different assembly) type reference.</summary>
    public class InstanceTypeDocumentationElement : TypeReferenceDocumentationElement
    {
        internal InstanceTypeDocumentationElement()
        {
        }

        /// <summary>The type name.</summary>
        public string Name { get; internal set; }

        /// <summary>The type namespace.</summary>
        public string Namespace { get; internal set; }

        /// <summary>The type generic arguments.</summary>
        public IReadOnlyList<TypeReferenceDocumentationElement> GenericArguments { get; internal set; }

        /// <summary>The declaring type, if any.</summary>
        public InstanceTypeDocumentationElement DeclaringType { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyReferenceDocumentationElement Assembly { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
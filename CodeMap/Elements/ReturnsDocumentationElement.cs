using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a returns sections corresponding to the <c>returns</c> XML element.</summary>
    public sealed class ReturnsDocumentationElement : DocumentationElement
    {
        internal ReturnsDocumentationElement()
        {
        }

        /// <summary>The return type.</summary>
        public TypeReferenceData Type { get; internal set; }

        /// <summary>The return attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The content of the returns section.</summary>
        public IReadOnlyList<BlockDocumentationElement> Description { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitReturnsBeginning(Type);
            foreach (var block in Description)
                block.Accept(visitor);
            visitor.VisitReturnsEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitReturnsBeginningAsync(Type, cancellationToken);
            foreach (var block in Description)
                await block.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitReturnsEndingAsync(cancellationToken);
        }
    }
}
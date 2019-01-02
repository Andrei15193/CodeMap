using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a remarks section corresponding to the <c>remarks</c> XML element.</summary>
    public sealed class RemarksDocumentationElement : DocumentationElement
    {
        internal RemarksDocumentationElement(IEnumerable<BlockDocumentationElement> content)
        {
            Content = content as IReadOnlyList<BlockDocumentationElement>
                ?? content?.ToList()
                ?? throw new ArgumentNullException(nameof(content));
            if (Content.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(content));
        }

        /// <summary>The content of the remarks section.</summary>
        public IReadOnlyList<BlockDocumentationElement> Content { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitRemarksBeginning();
            foreach (var block in Content)
                block.Accept(visitor);
            visitor.VisitRemarksEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitRemarksBeginningAsync(cancellationToken).ConfigureAwait(false);
            foreach (var block in Content)
                await block.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitRemarksEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a table column corresponding to the <c>term</c> XML element inside a <c>listheader</c> XML element.</summary>
    public sealed class TableColumnDocumentationElement : DocumentationElement
    {
        internal TableColumnDocumentationElement(IEnumerable<InlineDocumentationElement> name)
        {
            Name = name as IReadOnlyList<InlineDocumentationElement>
                ?? name?.ToList()
                ?? throw new ArgumentNullException(nameof(name));
            if (Name.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(name));
        }

        /// <summary>The name of the column.</summary>
        public IReadOnlyList<InlineDocumentationElement> Name { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitTableColumnBeginning();
            foreach (var contentElement in Name)
                contentElement.Accept(visitor);
            visitor.VisitTableColumnEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitTableColumnBeginningAsync(cancellationToken).ConfigureAwait(false);
            foreach (var contentElement in Name)
                await contentElement.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitTableColumnEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
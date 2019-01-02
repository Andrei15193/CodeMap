using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a table row corresponding to the <c>item</c> XML element.</summary>
    public sealed class TableRowDocumentationElement : DocumentationElement
    {
        internal TableRowDocumentationElement(IEnumerable<TableCellDocumentationElement> cells)
        {
            Cells = cells as IReadOnlyList<TableCellDocumentationElement>
                ?? cells?.ToList()
                ?? throw new ArgumentNullException(nameof(cells));
            if (Cells.Contains(null))
                throw new ArgumentException("Cannot contain 'null' cells.", nameof(cells));
        }

        /// <summary>The cells that form the table row.</summary>
        public IReadOnlyList<TableCellDocumentationElement> Cells { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitTableRowBeginning();
            foreach (var contentElement in Cells)
                contentElement.Accept(visitor);
            visitor.VisitTableRowEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitTableRowBeginningAsync(cancellationToken).ConfigureAwait(false);
            foreach (var cell in Cells)
                await cell.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitTableRowEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
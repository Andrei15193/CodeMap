using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a table row corresponding to the <c>item</c> XML element.</summary>
    public sealed class TableRowDocumentationElement : DocumentationElement
    {
        internal TableRowDocumentationElement(IEnumerable<TableCellDocumentationElement> cells, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Cells = cells as IReadOnlyList<TableCellDocumentationElement>
                ?? cells?.ToList()
                ?? throw new ArgumentNullException(nameof(cells));
            if (Cells.Contains(null))
                throw new ArgumentException("Cannot contain 'null' cells.", nameof(cells));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The cells that form the table row.</summary>
        public IReadOnlyList<TableCellDocumentationElement> Cells { get; }

        /// <summary>The XML attributes specified on the table row element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitTableRowBeginning(XmlAttributes);
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
            await visitor.VisitTableRowBeginningAsync(XmlAttributes, cancellationToken).ConfigureAwait(false);
            foreach (var cell in Cells)
                await cell.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitTableRowEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
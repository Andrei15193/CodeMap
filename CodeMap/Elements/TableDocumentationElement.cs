using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a table element corresponding to a <c>list</c> XML element where the <c>type</c> attribute is <c>table</c>.</summary>
    /// <remarks>
    /// <para>
    /// The returned table is normalized in the sense that if there were more columns or rows with missing cells they will be filled with
    /// empty ones so that the table has equal number of columns for each row, including the header.
    /// </para>
    /// </remarks>
    public sealed class TableDocumentationElement : BlockDocumentationElement
    {
        internal TableDocumentationElement(IEnumerable<TableColumnDocumentationElement> columns, IEnumerable<TableRowDocumentationElement> rows, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            var columnsList = columns as IReadOnlyList<TableColumnDocumentationElement>
                ?? columns?.ToList()
                ?? throw new ArgumentNullException(nameof(columns));
            if (columnsList.Contains(null))
                throw new ArgumentException("Cannot contain 'null' columns.", nameof(columns));

            var rowsList = rows as IReadOnlyList<TableRowDocumentationElement>
                ?? rows?.ToList()
                ?? throw new ArgumentNullException(nameof(rows));
            if (rowsList.Contains(null))
                throw new ArgumentException("Cannot contain 'null' rows.", nameof(rows));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));

            var actualColumnCount = Math.Max(columnsList.Count, rowsList.Max(row => (int?)row.Cells.Count) ?? 0);

            Columns = columnsList.Count == actualColumnCount
                ? columnsList
                : columnsList
                    .Concat(
                        Enumerable.Repeat(
                            TableColumn(Enumerable.Empty<InlineDocumentationElement>()),
                            actualColumnCount - columnsList.Count
                        )
                    )
                    .ToList();
            Rows = _EnsureRowCellsCount(rowsList, actualColumnCount);
        }

        internal TableDocumentationElement(IEnumerable<TableRowDocumentationElement> rows, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            var rowsList = rows as IReadOnlyList<TableRowDocumentationElement>
                ?? rows?.ToList()
                ?? throw new ArgumentNullException(nameof(rows));
            if (rowsList.Contains(null))
                throw new ArgumentException("Cannot contain 'null' rows.", nameof(rows));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));

            var actualColumnCount = rowsList.Max(row => (int?)row.Cells.Count) ?? 0;
            Columns = Enumerable.Empty<TableColumnDocumentationElement>() as IReadOnlyList<TableColumnDocumentationElement>
                ?? new List<TableColumnDocumentationElement>();
            Rows = _EnsureRowCellsCount(rowsList, actualColumnCount);
        }

        private static IReadOnlyList<TableRowDocumentationElement> _EnsureRowCellsCount(IReadOnlyList<TableRowDocumentationElement> rowsList, int columnCount)
        {
            var emptyCell = TableCell(Enumerable.Empty<InlineDocumentationElement>());
            return rowsList
                .Select(
                    row => row.Cells.Count >= columnCount
                        ? row
                        : TableRow(
                            row.Cells.Concat(Enumerable.Repeat(emptyCell, columnCount - row.Cells.Count)),
                            row.XmlAttributes
                        )
                )
                .ToList();
        }

        /// <summary>The columns of the table.</summary>
        public IReadOnlyList<TableColumnDocumentationElement> Columns { get; }

        /// <summary>The rows of the table.</summary>
        public IReadOnlyList<TableRowDocumentationElement> Rows { get; }

        /// <summary>The XML attributes specified on the table element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitTableBeginning(XmlAttributes);

            if (Columns.Count > 0)
            {
                visitor.VisitTableHeadingBeginning();
                foreach (var column in Columns)
                    column.Accept(visitor);
                visitor.VisitTableHeadingEnding();
            }
            if (Rows.Count > 0)
            {
                visitor.VisitTableBodyBeginning();
                foreach (var row in Rows)
                    row.Accept(visitor);
                visitor.VisitTableBodyEnding();
            }

            visitor.VisitTableEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitTableBeginningAsync(XmlAttributes, cancellationToken).ConfigureAwait(false);

            if (Columns.Count > 0)
            {
                await visitor.VisitTableHeadingBeginningAsync(cancellationToken).ConfigureAwait(false);
                foreach (var column in Columns)
                    await column.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
                await visitor.VisitTableHeadingEndingAsync(cancellationToken).ConfigureAwait(false);
            }
            if (Rows.Count > 0)
            {
                await visitor.VisitTableBodyBeginningAsync(cancellationToken).ConfigureAwait(false);
                foreach (var row in Rows)
                    await row.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
                await visitor.VisitTableBodyEndingAsync(cancellationToken).ConfigureAwait(false);
            }

            await visitor.VisitTableEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
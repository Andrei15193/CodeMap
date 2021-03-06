﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
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
            var columnsList = columns.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(columns));
            if (columnsList.Contains(null))
                throw new ArgumentException("Cannot contain 'null' columns.", nameof(columns));

            var rowsList = rows.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(rows));
            if (rowsList.Contains(null))
                throw new ArgumentException("Cannot contain 'null' rows.", nameof(rows));

            XmlAttributes = xmlAttributes ?? Extensions.EmptyDictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value is null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));

            var actualColumnCount = Math.Max(columnsList.Count, rowsList.Max(row => (int?)row.Cells.Count) ?? 0);

            Columns = columnsList.Count == actualColumnCount
                ? columnsList
                : columnsList
                    .Concat(
                        Enumerable.Repeat(
                            TableColumn(Array.Empty<InlineDocumentationElement>()),
                            actualColumnCount - columnsList.Count
                        )
                    )
                    .ToReadOnlyList();
            Rows = _EnsureRowCellsCount(rowsList, actualColumnCount);
        }

        internal TableDocumentationElement(IEnumerable<TableRowDocumentationElement> rows, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            var rowsList = rows.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(rows));
            if (rowsList.Contains(null))
                throw new ArgumentException("Cannot contain 'null' rows.", nameof(rows));

            XmlAttributes = xmlAttributes ?? Extensions.EmptyDictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value is null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));

            var actualColumnCount = rowsList.Max(row => (int?)row.Cells.Count) ?? 0;
            Columns = Array.Empty<TableColumnDocumentationElement>();
            Rows = _EnsureRowCellsCount(rowsList, actualColumnCount);
        }

        private static IReadOnlyList<TableRowDocumentationElement> _EnsureRowCellsCount(IReadOnlyList<TableRowDocumentationElement> rowsList, int columnCount)
        {
            var emptyCell = TableCell(Array.Empty<InlineDocumentationElement>());
            return rowsList
                .Select(
                    row => row.Cells.Count >= columnCount
                        ? row
                        : TableRow(
                            row.Cells.Concat(Enumerable.Repeat(emptyCell, columnCount - row.Cells.Count)),
                            row.XmlAttributes
                        )
                )
                .ToReadOnlyList();
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
            => visitor.VisitTable(this);
    }
}
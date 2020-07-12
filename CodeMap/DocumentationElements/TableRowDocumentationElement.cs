using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a table row corresponding to the <c>item</c> XML element.</summary>
    public sealed class TableRowDocumentationElement : DocumentationElement
    {
        internal TableRowDocumentationElement(IEnumerable<TableCellDocumentationElement> cells, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Cells = cells.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(cells));
            if (Cells.Contains(null))
                throw new ArgumentException("Cannot contain 'null' cells.", nameof(cells));

            XmlAttributes = xmlAttributes ?? Extensions.EmptyDictionary<string, string>();
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
            => visitor.VisitTableRow(this);
    }
}
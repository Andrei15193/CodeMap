using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a table column corresponding to the <c>term</c> XML element inside a <c>listheader</c> XML element.</summary>
    public sealed class TableColumnDocumentationElement : DocumentationElement
    {
        internal TableColumnDocumentationElement(IEnumerable<InlineDocumentationElement> name, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Name = name as IReadOnlyList<InlineDocumentationElement>
                ?? name?.ToList()
                ?? throw new ArgumentNullException(nameof(name));
            if (Name.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(name));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The name of the column.</summary>
        public IReadOnlyList<InlineDocumentationElement> Name { get; }

        /// <summary>The XML attributes specified on the table column element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitTableColumnBeginning(XmlAttributes);
            foreach (var contentElement in Name)
                contentElement.Accept(visitor);
            visitor.VisitTableColumnEnding();
        }
    }
}
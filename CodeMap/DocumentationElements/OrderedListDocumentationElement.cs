using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents an order list corresponding to the <c>list</c> XML element where the <c>type</c> attribute is <c>number</c>.</summary>
    public sealed class OrderedListDocumentationElement : BlockDocumentationElement
    {
        internal OrderedListDocumentationElement(IEnumerable<ListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Items = items as IReadOnlyList<ListItemDocumentationElement>
                ?? items?.ToList()
                ?? throw new ArgumentNullException(nameof(items));
            if (Items.Contains(null))
                throw new ArgumentException("Cannot contain 'null' items.", nameof(items));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The items forming the ordered list.</summary>
        public IReadOnlyList<ListItemDocumentationElement> Items { get; }

        /// <summary>The XML attributes specified on the ordered list element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitOrderedList(this);
    }
}
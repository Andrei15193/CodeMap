using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents an unordered list corresponding to the <c>list</c> XML element where the <c>type</c> attribute is <c>bullet</c> or missing.</summary>
    public sealed class UnorderedListDocumentationElement : BlockDocumentationElement
    {
        internal UnorderedListDocumentationElement(IEnumerable<ListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
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

        /// <summary>The items that form the unordered list.</summary>
        public IReadOnlyList<ListItemDocumentationElement> Items { get; }

        /// <summary>The XML attributes specified on the unordered list element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitUnorderedList(this);
    }
}
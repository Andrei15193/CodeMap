using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a definition list element corresponding to the <c>list</c> XML element.</summary>
    public sealed class DefinitionListDocumentationElement : BlockDocumentationElement
    {
        internal DefinitionListDocumentationElement(DefinitionListTitleDocumentationElement listTitle, IEnumerable<DefinitionListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            ListTitle = listTitle;

            Items = items.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(items));
            if (Items.Contains(null))
                throw new ArgumentException("Cannot contain 'null' items.", nameof(items));

            XmlAttributes = xmlAttributes ?? Extensions.EmptyDictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value is null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        internal DefinitionListDocumentationElement(IEnumerable<DefinitionListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
            : this(null, items, xmlAttributes)
        {
        }

        /// <summary>The list title of the definition list.</summary>
        public DefinitionListTitleDocumentationElement ListTitle { get; }

        /// <summary>The items that form the definition list.</summary>
        public IReadOnlyList<DefinitionListItemDocumentationElement> Items { get; }

        /// <summary>The XML attributes specified on the definition list element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitDefinitionList(this);
    }
}
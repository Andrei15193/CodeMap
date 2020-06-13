using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a definition list item corresponding to the <c>item</c> XML element containing both a <c>term</c> and a <c>description</c> XML element.</summary>
    public sealed class DefinitionListItemDocumentationElement : DocumentationElement
    {
        internal DefinitionListItemDocumentationElement(InlineDescriptionDocumentationElement term, InlineDescriptionDocumentationElement description, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Term = term ?? throw new ArgumentNullException(nameof(term));
            Description = description ?? throw new ArgumentNullException(nameof(description));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The defined term.</summary>
        public InlineDescriptionDocumentationElement Term { get; }

        /// <summary>The term description or definition.</summary>
        public InlineDescriptionDocumentationElement Description { get; }

        /// <summary>The additional XML attributes on the definition list item element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitDefinitionListItem(this);
    }
}
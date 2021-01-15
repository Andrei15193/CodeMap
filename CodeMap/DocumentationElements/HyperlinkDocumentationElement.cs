using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents an inline hyperlink that is part of a <see cref="BlockDocumentationElement"/>.</summary>
    public class HyperlinkDocumentationElement : InlineDocumentationElement
    {
        internal HyperlinkDocumentationElement(string destination, string text, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));
            Text = text ?? throw new ArgumentNullException(nameof(text));

            XmlAttributes = xmlAttributes ?? Extensions.EmptyDictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value is null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The hyperlink destination (URL).</summary>
        public string Destination { get; }

        /// <summary>The hyperlink text.</summary>
        new public string Text { get; }

        /// <summary>The XML attributes specified on the inline code element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitHyperlink(this);
    }
}
using System;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents an inline hyperlink that is part of a <see cref="BlockDocumentationElement"/>.</summary>
    public class HyperlinkDocumentationElement : InlineDocumentationElement
    {
        internal HyperlinkDocumentationElement(string destination, string text)
        {
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>The hyperlink destination (URL).</summary>
        public string Destination { get; }

        /// <summary>The hyperlink text.</summary>
        new public string Text { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitHyperlink(this);
    }
}
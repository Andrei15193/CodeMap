using System;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents inline text that is part of a <see cref="BlockDocumentationElement"/>.</summary>
    public sealed class TextDocumentationElement : InlineDocumentationElement
    {
        internal TextDocumentationElement(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>The plain text.</summary>
        new public string Text { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitText(Text);
    }
}
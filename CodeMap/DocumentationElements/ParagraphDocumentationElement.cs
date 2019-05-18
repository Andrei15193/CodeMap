using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a paragraph corresponding to the <c>para</c> XML element.</summary>
    public sealed class ParagraphDocumentationElement : BlockDocumentationElement
    {
        internal ParagraphDocumentationElement(IEnumerable<InlineDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Content = content as IReadOnlyList<InlineDocumentationElement>
                ?? content?.ToList()
                ?? throw new ArgumentNullException(nameof(content));
            if (Content.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(content));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The content of the paragraph.</summary>
        public IReadOnlyList<InlineDocumentationElement> Content { get; }

        /// <summary>The XML attributes specified on the paragraph element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitParagraphBeginning(XmlAttributes);
            foreach (var contentElement in Content)
                contentElement.Accept(visitor);
            visitor.VisitParagraphEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitParagraphBeginningAsync(XmlAttributes, cancellationToken).ConfigureAwait(false);
            foreach (var contentElement in Content)
                await contentElement.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitParagraphEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
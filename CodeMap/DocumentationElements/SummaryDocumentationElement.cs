using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a summary section corresponding to the <c>summary</c> XML element.</summary>
    public sealed class SummaryDocumentationElement : DocumentationElement
    {
        internal SummaryDocumentationElement(IEnumerable<BlockDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Content = content as IReadOnlyList<BlockDocumentationElement>
                ?? content?.ToList()
                ?? throw new ArgumentNullException(nameof(content));
            if (Content.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(content));
            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The content of the summary section.</summary>
        public IReadOnlyList<BlockDocumentationElement> Content { get; }

        /// <summary>The XML attributes specified on the summary element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitSummaryBeginning(XmlAttributes);
            foreach (var block in Content)
                block.Accept(visitor);
            visitor.VisitSummaryEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitSummaryBeginningAsync(XmlAttributes, cancellationToken).ConfigureAwait(false);
            foreach (var block in Content)
                await block.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitSummaryEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
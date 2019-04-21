using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a value section corresponding to the <c>value</c> XML element.</summary>
    public sealed class ValueDocumentationElement : DocumentationElement
    {
        internal ValueDocumentationElement(IEnumerable<BlockDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
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

        /// <summary>The content describing the value.</summary>
        public IReadOnlyList<BlockDocumentationElement> Content { get; }

        /// <summary>The additional XML attributes on the value element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitValueBeginning(XmlAttributes);
            foreach (var block in Content)
                block.Accept(visitor);
            visitor.VisitValueEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitValueBeginningAsync(XmlAttributes, cancellationToken).ConfigureAwait(false);
            foreach (var block in Content)
                await block.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitValueEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
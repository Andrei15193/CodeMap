using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>
    /// Represents an example corresponding to the <c>example</c> XML element.
    /// </summary>
    public sealed class ExampleDocumentationElement : DocumentationElement
    {
        internal ExampleDocumentationElement(IEnumerable<BlockDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
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

        /// <summary>The content inside the example.</summary>
        public IReadOnlyList<BlockDocumentationElement> Content { get; }

        /// <summary>The additional XML attributes on the example element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitExampleBeginning();
            foreach (var block in Content)
                block.Accept(visitor);
            visitor.VisitExampleEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitExampleBeginningAsync(cancellationToken).ConfigureAwait(false);
            foreach (var block in Content)
                await block.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitExampleEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents an order list corresponding to the <c>list</c> XML element where the <c>type</c> attribute is <c>number</c>.</summary>
    public sealed class OrderedListDocumentationElement : BlockDocumentationElement
    {
        internal OrderedListDocumentationElement(IEnumerable<ListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
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

        /// <summary>The items forming the ordered list.</summary>
        public IReadOnlyList<ListItemDocumentationElement> Items { get; }

        /// <summary>The XML attributes specified on the ordered list element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitOrderedListBeginning();
            foreach (var item in Items)
                item.Accept(visitor);
            visitor.VisitOrderedListEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitOrderedListBeginningAsync(cancellationToken).ConfigureAwait(false);
            foreach (var item in Items)
                await item.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitOrderedListEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
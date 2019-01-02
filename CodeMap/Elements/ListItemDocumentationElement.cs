﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a list item corresponding to the <c>item</c> XML element.</summary>
    public sealed class ListItemDocumentationElement : DocumentationElement
    {
        internal ListItemDocumentationElement(IEnumerable<InlineDocumentationElement> content)
        {
            Content = content as IReadOnlyList<InlineDocumentationElement>
                ?? content?.ToList()
                ?? throw new ArgumentNullException(nameof(content));
            if (Content.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(content));
        }

        /// <summary>The content of the list item.</summary>
        public IReadOnlyList<InlineDocumentationElement> Content { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitListItemBeginning();
            foreach (var contentElement in Content)
                contentElement.Accept(visitor);
            visitor.VisitListItemEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitListItemBeginningAsync(cancellationToken).ConfigureAwait(false);
            foreach (var contentElement in Content)
                await contentElement.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitListItemEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
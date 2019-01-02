using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a definition list element corresponding to the <c>list</c> XML element.</summary>
    public sealed class DefinitionListDocumentationElement : BlockDocumentationElement
    {
        internal DefinitionListDocumentationElement(IEnumerable<InlineDocumentationElement> listTitle, IEnumerable<DefinitionListItemDocumentationElement> items)
        {
            ListTitle = listTitle as IReadOnlyList<InlineDocumentationElement>
                ?? listTitle?.ToList();
            if (ListTitle != null && ListTitle.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(listTitle));

            Items = items as IReadOnlyList<DefinitionListItemDocumentationElement>
                ?? items?.ToList()
                ?? throw new ArgumentNullException(nameof(items));
            if (Items.Contains(null))
                throw new ArgumentException("Cannot contain 'null' items.", nameof(items));
        }

        internal DefinitionListDocumentationElement(IEnumerable<DefinitionListItemDocumentationElement> items)
            : this(null, items)
        {
        }

        /// <summary>The list title of the definition list.</summary>
        public IReadOnlyList<InlineDocumentationElement> ListTitle { get; }

        /// <summary>The items that form the definition list.</summary>
        public IReadOnlyList<DefinitionListItemDocumentationElement> Items { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitDefinitionListBeginning();
            if (ListTitle != null)
            {
                visitor.VisitDefinitionListTitleBeginning();
                foreach (var contentElement in ListTitle)
                    contentElement.Accept(visitor);
                visitor.VisitDefinitionListTitleEnding();
            }
            foreach (var item in Items)
                item.Accept(visitor);
            visitor.VisitDefinitionListEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitDefinitionListBeginningAsync(cancellationToken).ConfigureAwait(false);
            if (ListTitle != null)
            {
                await visitor.VisitDefinitionListTitleBeginningAsync(cancellationToken).ConfigureAwait(false);
                foreach (var contentElement in ListTitle)
                    await contentElement.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
                await visitor.VisitDefinitionListTitleEndingAsync(cancellationToken).ConfigureAwait(false);
            }
            foreach (var item in Items)
                await item.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitDefinitionListEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
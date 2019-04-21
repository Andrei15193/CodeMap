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
        internal DefinitionListDocumentationElement(InlineDescriptionDocumentationElement listTitle, IEnumerable<DefinitionListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            ListTitle = listTitle ?? InlineDescription();
            if (ListTitle.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(listTitle));

            Items = items as IReadOnlyList<DefinitionListItemDocumentationElement>
                ?? items?.ToList()
                ?? throw new ArgumentNullException(nameof(items));
            if (Items.Contains(null))
                throw new ArgumentException("Cannot contain 'null' items.", nameof(items));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        internal DefinitionListDocumentationElement(IEnumerable<DefinitionListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
            : this(null, items, xmlAttributes)
        {
        }

        /// <summary>The list title of the definition list.</summary>
        public InlineDescriptionDocumentationElement ListTitle { get; }

        /// <summary>The items that form the definition list.</summary>
        public IReadOnlyList<DefinitionListItemDocumentationElement> Items { get; }

        /// <summary>The XML attributes specified on the definition list element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitDefinitionListBeginning(XmlAttributes);
            if (ListTitle.Count > 0)
            {
                visitor.VisitDefinitionListTitleBeginning(ListTitle.XmlAttributes);
                ListTitle.Accept(visitor);
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
            await visitor.VisitDefinitionListBeginningAsync(XmlAttributes, cancellationToken).ConfigureAwait(false);
            if (ListTitle.Count > 0)
            {
                await visitor.VisitDefinitionListTitleBeginningAsync(ListTitle.XmlAttributes, cancellationToken).ConfigureAwait(false);
                await ListTitle.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
                await visitor.VisitDefinitionListTitleEndingAsync(cancellationToken).ConfigureAwait(false);
            }
            foreach (var item in Items)
                await item.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitDefinitionListEndingAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
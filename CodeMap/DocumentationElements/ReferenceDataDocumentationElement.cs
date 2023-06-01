using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.ReferenceData;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a resolved member reference corresponding to the <c>see</c> and <c>seealso</c> XML elements.</summary>
    public sealed class ReferenceDataDocumentationElement : MemberReferenceDocumentationElement
    {
        internal ReferenceDataDocumentationElement(MemberReference referredMember, IEnumerable<InlineDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            ReferredMember = referredMember ?? throw new ArgumentNullException(nameof(referredMember));
            Content = content.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(content));
            if (Content.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(content));

            XmlAttributes = xmlAttributes ?? Extensions.EmptyDictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value is null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The referred member.</summary>
        public MemberReference ReferredMember { get; }

        /// <summary>The content of the member reference.</summary>
        public IReadOnlyList<InlineDocumentationElement> Content { get; }

        /// <summary>The XML attributes specified on the member reference element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitInlineReference(this);
    }
}
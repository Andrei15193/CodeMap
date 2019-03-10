using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a resolved member reference corresponding to the <c>see</c> and <c>seealso</c> XML elements.</summary>
    public sealed class MemberInfoReferenceDocumentationElement : MemberReferenceDocumentationElement
    {
        internal MemberInfoReferenceDocumentationElement(MemberInfo referredMember, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            ReferredMember = referredMember ?? throw new ArgumentNullException(nameof(referredMember));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The referred member.</summary>
        public MemberInfo ReferredMember { get; }

        /// <summary>The XML attributes specified on the member reference element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitInlineReference(ReferredMember);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitInlineReferenceAsync(ReferredMember, cancellationToken);
    }
}
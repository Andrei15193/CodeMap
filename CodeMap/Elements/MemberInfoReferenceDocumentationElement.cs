using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a resolved member reference corresponding to the <c>see</c> and <c>seealso</c> XML elements.</summary>
    public sealed class MemberInfoReferenceDocumentationElement : MemberReferenceDocumentationElement
    {
        internal MemberInfoReferenceDocumentationElement(MemberInfo referredMember)
        {
            ReferredMember = referredMember ?? throw new ArgumentNullException(nameof(referredMember));
        }

        /// <summary>The referred member.</summary>
        public MemberInfo ReferredMember { get; }

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
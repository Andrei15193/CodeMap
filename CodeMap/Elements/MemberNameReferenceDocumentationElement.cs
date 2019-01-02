﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents an unresolved member reference corresponding to the <c>see</c> and <c>seealso</c> XML elements.</summary>
    public sealed class MemberNameReferenceDocumentationElement : MemberReferenceDocumentationElement
    {
        internal MemberNameReferenceDocumentationElement(string canonicalName)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
                throw new ArgumentException("Cannot be 'null', empty or white space.", nameof(canonicalName));
            CanonicalName = canonicalName;
        }

        /// <summary>The canonical name of the referred member.</summary>
        public string CanonicalName { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitInlineReference(CanonicalName);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitInlineReferenceAsync(CanonicalName, cancellationToken);
    }
}
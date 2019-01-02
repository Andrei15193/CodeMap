﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a generic parameter reference corresponding to the <c>typeparamref</c> XML element.</summary>
    public sealed class GenericParameterReferenceDocumentationElement : InlineDocumentationElement
    {
        internal GenericParameterReferenceDocumentationElement(string genericParameterName)
        {
            GenericParameterName = genericParameterName ?? throw new ArgumentNullException(nameof(genericParameterName));
        }

        /// <summary>The name of the referred generic parameter.</summary>
        public string GenericParameterName { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException(nameof(visitor));
            visitor.VisitGenericParameterReference(GenericParameterName);
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitGenericParameterReferenceAsync(GenericParameterName, cancellationToken);
    }
}
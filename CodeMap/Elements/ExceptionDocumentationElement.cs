using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented exception that might be thrown by a method.</summary>
    public sealed class ExceptionDocumentationElement : DocumentationElement
    {
        internal ExceptionDocumentationElement()
        {
        }

        /// <summary>The type of the exception.</summary>
        public TypeReferenceData Type { get; internal set; }

        /// <summary>The description of the cases where the exception is thrown.</summary>
        public IReadOnlyList<BlockDocumentationElement> Description { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            throw new NotImplementedException();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
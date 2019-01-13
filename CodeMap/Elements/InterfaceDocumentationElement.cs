using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented interface declaration.</summary>
    public sealed class InterfaceDocumentationElement : TypeDocumentationElement
    {
        internal InterfaceDocumentationElement()
        {
        }

        /// <summary>The interface generic parameters.</summary>
        public IReadOnlyList<GenericParameterDocumentationElement> GenericParameters { get; internal set; }

        /// <summary>The base interfaces.</summary>
        public IReadOnlyCollection<TypeReferenceDocumentationElement> BaseInterfaces { get; internal set; }

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
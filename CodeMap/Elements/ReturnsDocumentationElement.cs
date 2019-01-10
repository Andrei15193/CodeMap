using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a returns sections corresponding to the <c>returns</c> XML element.</summary>
    public sealed class ReturnsDocumentationElement : DocumentationElement
    {
        internal ReturnsDocumentationElement(TypeReferenceDocumentationElement typeReferenceData, IEnumerable<AttributeData> attributes, IEnumerable<BlockDocumentationElement> description)
        {
            ReturnType = typeReferenceData ?? throw new ArgumentNullException(nameof(typeReferenceData));
            if (ReturnType is VoidTypeReferenceDocumentationElement)
                throw new ArgumentException("Cannot be 'void' return type.", nameof(typeReferenceData));

            Attributes = attributes.AsReadOnlyCollection()
                ?? throw new ArgumentNullException(nameof(attributes));
            if (Attributes.Contains(null))
                throw new ArgumentException("Cannot contain 'null' attributes.", nameof(attributes));

            Description = description.AsReadOnlyList()
                ?? throw new ArgumentNullException(nameof(description));
            if (Description.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(description));
        }

        /// <summary>The return type.</summary>
        public TypeReferenceDocumentationElement ReturnType { get; }

        /// <summary>The return attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; }

        /// <summary>The content of the returns section.</summary>
        public IReadOnlyList<BlockDocumentationElement> Description { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitReturnsBeginning(ReturnType);
            foreach (var block in Description)
                block.Accept(visitor);
            visitor.VisitReturnsEnding();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitReturnsBeginningAsync(ReturnType, cancellationToken);
            foreach (var block in Description)
                await block.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await visitor.VisitReturnsEndingAsync(cancellationToken);
        }
    }
}
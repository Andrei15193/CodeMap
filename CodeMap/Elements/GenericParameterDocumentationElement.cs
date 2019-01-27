using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented generic parameter.</summary>
    public abstract class GenericParameterDocumentationElement : TypeReferenceDocumentationElement
    {
        internal GenericParameterDocumentationElement()
        {
        }

        /// <summary>The name of the generic parameter.</summary>
        public string Name { get; internal set; }

        /// <summary>The position of the generic parameter.</summary>
        public int Position { get; internal set; }

        /// <summary>Indicates whether the parameter is covariant.</summary>
        public bool IsCovariant { get; internal set; }

        /// <summary>Indicates whether the parameter is contravariant.</summary>
        public bool IsContravariant { get; internal set; }

        /// <summary>Indicates whether the generic argument must be a reference type.</summary>
        public bool HasReferenceTypeConstraint { get; internal set; }

        /// <summary>Indicates whether the generic argument must be a non nullable value type.</summary>
        public bool HasNonNullableValueTypeConstraint { get; internal set; }

        /// <summary>Indicates whether the generic argument must have a public parameterless constructor.</summary>
        public bool HasDefaultConstructorConstraint { get; internal set; }

        /// <summary>The generic argument type constraints (base class, implemented interfaces, generic argument inheritance).</summary>
        public IReadOnlyCollection<TypeReferenceDocumentationElement> TypeConstraints { get; internal set; }

        /// <summary>The generic parameter description.</summary>
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
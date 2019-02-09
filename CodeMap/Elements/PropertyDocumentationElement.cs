using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented property declared by a type.</summary>
    public sealed class PropertyDocumentationElement : MemberDocumentationElement
    {
        internal PropertyDocumentationElement()
        {
        }

        /// <summary>The property type.</summary>
        public TypeReferenceDocumentationElement Type { get; internal set; }

        /// <summary>Indicates whether the property is static.</summary>
        public bool IsStatic { get; internal set; }

        /// <summary>Indicates whether the property has been marked as virtual.</summary>
        public bool IsVirtual { get; internal set; }

        /// <summary>Indicates whether the property has been marked as abstract.</summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>Indicates whether the property is an override.</summary>
        public bool IsOverride { get; internal set; }

        /// <summary>Indicates whether the property has been marked as sealed.</summary>
        public bool IsSealed { get; internal set; }

        /// <summary>Indicates whether the property hides a member from a base type.</summary>
        public bool IsShadowing { get; internal set; }

        /// <summary>Information about the getter accessor.</summary>
        public PropertyAccessorData Getter { get; internal set; }

        /// <summary>Information about the setter accessor.</summary>
        public PropertyAccessorData Setter { get; internal set; }

        /// <summary>The method parameters.</summary>
        public IReadOnlyList<ParameterDocumentationElement> Parameters { get; internal set; }

        /// <summary>Documentation about the how the value of the property is calculated.</summary>
        new public ValueDocumentationElement Value { get; internal set; }

        /// <summary>Documented exceptions that might be thrown when using the property.</summary>
        public IReadOnlyCollection<ExceptionDocumentationElement> Exceptions { get; internal set; }

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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented constant declared by a type.</summary>
    public class FieldDocumentationElement : MemberDocumentationElement
    {
        internal FieldDocumentationElement()
        {
        }

        /// <summary>The field type.</summary>
        public TypeReferenceData Type { get; internal set; }

        /// <summary>Indicates whether the field is static.</summary>
        public bool IsStatic { get; internal set; }

        /// <summary>Indicates whether the field is read only.</summary>
        public bool IsReadOnly { get; internal set; }

        /// <summary>Indicates whether the field hides a member from a base type.</summary>
        public bool IsShadowing { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitField(this);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitFieldAsync(this, cancellationToken);
    }
}
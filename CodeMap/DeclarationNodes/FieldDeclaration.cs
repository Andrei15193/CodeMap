using CodeMap.ReferenceData;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented constant declared by a type.</summary>
    public class FieldDeclaration : MemberDeclaration
    {
        internal FieldDeclaration()
        {
        }

        /// <summary>The field type.</summary>
        public BaseTypeReference Type { get; internal set; }

        /// <summary>Indicates whether the field is static.</summary>
        public bool IsStatic { get; internal set; }

        /// <summary>Indicates whether the field is read only.</summary>
        public bool IsReadOnly { get; internal set; }

        /// <summary>Indicates whether the field hides a member from a base type.</summary>
        public bool IsShadowing { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitField(this);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DeclarationNodeVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitFieldAsync(this, cancellationToken);
    }
}
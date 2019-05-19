using CodeMap.ReferenceData;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented enum declaration.</summary>
    public class EnumDeclaration : TypeDeclaration
    {
        internal EnumDeclaration()
        {
        }

        /// <summary>The underlying type of the enum members.</summary>
        public TypeReference UnderlyingType { get; internal set; }

        /// <summary>The enum members.</summary>
        public IReadOnlyList<ConstantDeclaration> Members { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitEnum(this);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DeclarationNodeVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitEnumAsync(this, cancellationToken);
    }
}
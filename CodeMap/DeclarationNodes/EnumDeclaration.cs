using CodeMap.ReferenceData;
using System.Collections.Generic;

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
    }
}
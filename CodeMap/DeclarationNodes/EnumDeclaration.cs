using System;
using System.Collections.Generic;
using CodeMap.ReferenceData;

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

        /// <summary>Determines whether the current <see cref="EnumDeclaration"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="EnumDeclaration"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => base.Equals(type)
            && type.IsEnum;
    }
}
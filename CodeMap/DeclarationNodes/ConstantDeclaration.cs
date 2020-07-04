using System;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented constant declared by a type.</summary>
    public class ConstantDeclaration : MemberDeclaration, IEquatable<ConstantReference>
    {
        internal ConstantDeclaration()
        {
        }

        /// <summary>Indicates whether the constant hides a member from a base type.</summary>
        public bool IsShadowing { get; internal set; }

        /// <summary>The constant value.</summary>
        public object Value { get; internal set; }

        /// <summary>The constant type.</summary>
        public BaseTypeReference Type { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitConstant(this);

        /// <summary>Determines whether the current <see cref="ConstantDeclaration"/> is equal to the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstantDeclaration"/> references the provided <paramref name="memberReference"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberReference memberReference)
            => memberReference is ConstantReference constantReference
            && Equals(constantReference);

        /// <summary>Determines whether the current <see cref="ConstantDeclaration"/> is equal to the provided <paramref name="constantReference"/>.</summary>
        /// <param name="constantReference">The <see cref="ConstantReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstantDeclaration"/> references the provided <paramref name="constantReference"/>; <c>false</c> otherwise.</returns>
        public bool Equals(ConstantReference constantReference)
            => constantReference != null
            && string.Equals(Name, constantReference.Name, StringComparison.OrdinalIgnoreCase)
            && DeclaringType == constantReference.DeclaringType;
    }
}
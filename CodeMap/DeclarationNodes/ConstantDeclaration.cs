using System;
using System.Reflection;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented constant declared by a type.</summary>
    public class ConstantDeclaration : MemberDeclaration, IEquatable<FieldInfo>
    {
        /// <summary>Determines whether the provided <paramref name="constantDeclaration"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="constantDeclaration">The <see cref="ConstantDeclaration"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(ConstantDeclaration constantDeclaration, MemberInfo memberInfo)
            => Equals(constantDeclaration, memberInfo);

        /// <summary>Determines whether the provided <paramref name="constantDeclaration"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <param name="constantDeclaration">The <see cref="ConstantDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(MemberInfo memberInfo, ConstantDeclaration constantDeclaration)
            => Equals(constantDeclaration, memberInfo);

        /// <summary>Determines whether the provided <paramref name="constantDeclaration"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="constantDeclaration">The <see cref="ConstantDeclaration"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(ConstantDeclaration constantDeclaration, MemberInfo memberInfo)
            => !Equals(constantDeclaration, memberInfo);

        /// <summary>Determines whether the provided <paramref name="constantDeclaration"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <param name="constantDeclaration">The <see cref="ConstantDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(MemberInfo memberInfo, ConstantDeclaration constantDeclaration)
            => !Equals(constantDeclaration, memberInfo);

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

        /// <summary>Determines whether the current <see cref="ConstantDeclaration"/> is equal to the provided <paramref name="fieldInfo"/>.</summary>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstantDeclaration"/> references the provided <paramref name="fieldInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(FieldInfo fieldInfo)
            => base.Equals(fieldInfo);

        /// <summary>Determines whether the current <see cref="ConstantDeclaration"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstantDeclaration"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="FieldInfo"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="FieldInfo"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public override bool Equals(object obj)
            => obj is FieldInfo fieldInfo ? Equals(fieldInfo) : false;

        /// <summary>Calculates the has code for the current <see cref="ConstantDeclaration"/>.</summary>
        /// <returns>Returns a hash code for the current instance.</returns>
        public override int GetHashCode()
            => base.GetHashCode();
    }
}
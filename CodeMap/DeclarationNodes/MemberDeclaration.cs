using System;
using System.Collections.Generic;
using System.Reflection;
using CodeMap.DocumentationElements;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented declared member of a type.</summary>
    public abstract class MemberDeclaration : DeclarationNode, IEquatable<MemberInfo>
    {
        /// <summary>Determines whether the provided <paramref name="memberDeclaration"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="memberDeclaration">The <see cref="MemberDeclaration"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(MemberDeclaration memberDeclaration, MemberInfo memberInfo)
            => Equals(memberDeclaration, memberInfo);

        /// <summary>Determines whether the provided <paramref name="memberDeclaration"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <param name="memberDeclaration">The <see cref="MemberDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(MemberInfo memberInfo, MemberDeclaration memberDeclaration)
            => Equals(memberDeclaration, memberInfo);

        /// <summary>Determines whether the provided <paramref name="memberDeclaration"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="memberDeclaration">The <see cref="MemberDeclaration"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(MemberDeclaration memberDeclaration, MemberInfo memberInfo)
            => !Equals(memberDeclaration, memberInfo);

        /// <summary>Determines whether the provided <paramref name="memberDeclaration"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <param name="memberDeclaration">The <see cref="MemberDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(MemberInfo memberInfo, MemberDeclaration memberDeclaration)
            => !Equals(memberDeclaration, memberInfo);

        internal MemberDeclaration()
        {
        }

        /// <summary>The member name.</summary>
        public string Name { get; internal set; }

        /// <summary>The member access modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The member declaring type.</summary>
        public TypeDeclaration DeclaringType { get; internal set; }

        /// <summary>The member attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The member summary.</summary>
        public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The member remarks.</summary>
        public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The member examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The related members of the declared member.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; internal set; }

        /// <summary>Determines whether the current <see cref="MemberDeclaration"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberDeclaration"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(MemberInfo memberInfo)
            => memberInfo != null
            && string.Equals(Name, memberInfo.Name, StringComparison.OrdinalIgnoreCase)
            && DeclaringType == memberInfo.DeclaringType;

        /// <summary>Determines whether the current <see cref="MemberDeclaration"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberDeclaration"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="MemberInfo"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="MemberInfo"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public override bool Equals(object obj)
            => obj is MemberInfo memberInfo ? Equals(memberInfo) : base.Equals(obj);

        /// <summary>Calculates the has code for the current <see cref="MemberDeclaration"/>.</summary>
        /// <returns>Returns a hash code for the current instance.</returns>
        public override int GetHashCode()
            => base.GetHashCode();
    }
}
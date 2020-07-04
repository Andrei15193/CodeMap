using System;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented constant declared by a type.</summary>
    public class FieldDeclaration : MemberDeclaration, IEquatable<FieldReference>
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

        /// <summary>Determines whether the current <see cref="FieldDeclaration"/> is equal to the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="FieldDeclaration"/> references the provided <paramref name="memberReference"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberReference memberReference)
            => memberReference is FieldReference fieldReference
            && Equals(fieldReference);

        /// <summary>Determines whether the current <see cref="FieldDeclaration"/> is equal to the provided <paramref name="fieldReference"/>.</summary>
        /// <param name="fieldReference">The <see cref="FieldReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="FieldDeclaration"/> references the provided <paramref name="fieldReference"/>; <c>false</c> otherwise.</returns>
        public bool Equals(FieldReference fieldReference)
            => fieldReference != null
            && string.Equals(Name, fieldReference.Name, StringComparison.OrdinalIgnoreCase)
            && DeclaringType == fieldReference.DeclaringType;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented constructor declared by a type.</summary>
    public class ConstructorDeclaration : MemberDeclaration, IEquatable<ConstructorReference>
    {
        internal ConstructorDeclaration()
        {
        }

        /// <summary>The constructor parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>Documented exceptions that might be thrown when calling the constructor.</summary>
        public IReadOnlyCollection<ExceptionData> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitConstructor(this);

        /// <summary>Determines whether the current <see cref="ConstructorDeclaration"/> is equal to the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstructorDeclaration"/> references the provided <paramref name="memberReference"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberReference memberReference)
            => memberReference is ConstructorReference constructorReference
            && Equals(constructorReference);

        /// <summary>Determines whether the current <see cref="ConstructorDeclaration"/> is equal to the provided <paramref name="constructorReference"/>.</summary>
        /// <param name="constructorReference">The <see cref="ConstructorReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstructorDeclaration"/> references the provided <paramref name="constructorReference"/>; <c>false</c> otherwise.</returns>
        public bool Equals(ConstructorReference constructorReference)
            => constructorReference != null
            && DeclaringType == constructorReference.DeclaringType
            && Parameters.Select(parameter => parameter.Type).SequenceEqual(constructorReference.ParameterTypes);
    }
}
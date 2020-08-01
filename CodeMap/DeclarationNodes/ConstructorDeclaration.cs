using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DocumentationElements;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented constructor declared by a type.</summary>
    public class ConstructorDeclaration : MemberDeclaration, IEquatable<ConstructorInfo>
    {
        internal ConstructorDeclaration()
        {
        }

        /// <summary>The constructor parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>Documented exceptions that might be thrown when calling the constructor.</summary>
        public IReadOnlyCollection<ExceptionData> Exceptions { get; internal set; }

        /// <summary>Determines whether the current <see cref="ConstructorDeclaration"/> is equal to the provided <paramref name="constructorInfo"/>.</summary>
        /// <param name="constructorInfo">The <see cref="ConstructorInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstructorDeclaration"/> references the provided <paramref name="constructorInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(ConstructorInfo constructorInfo)
            => constructorInfo != null
            && Parameters.Count == constructorInfo.GetParameters().Length
            && Parameters
                .Zip(constructorInfo.GetParameters(), (parameterType, parameter) => (ExpectedParameterType: parameterType.Type, ActualParameterType: parameter.ParameterType))
                .All(pair => pair.ExpectedParameterType == pair.ActualParameterType)
            && DeclaringType == constructorInfo.DeclaringType;

        /// <summary>Determines whether the current <see cref="ConstructorDeclaration"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstructorDeclaration"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => memberInfo is ConstructorInfo constructorInfo ? Equals(constructorInfo) : false;

        /// <summary>Determines whether the current <see cref="ConstructorDeclaration"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstructorDeclaration"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="MemberInfo"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="MemberInfo"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public override bool Equals(object obj)
            => obj is ConstructorInfo constructorInfo ? Equals(constructorInfo) : false;

        /// <summary>Calculates the has code for the current <see cref="ConstructorDeclaration"/>.</summary>
        /// <returns>Returns a hash code for the current instance.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitConstructor(this);
    }
}
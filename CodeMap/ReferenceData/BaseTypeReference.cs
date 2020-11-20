using System;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a base class for type reference such as concrete types, generic type definitions, arrays and so on.</summary>
    public abstract class BaseTypeReference : MemberReference
    {
        internal BaseTypeReference()
        {
        }

        /// <summary>Determines whether the current <see cref="BaseTypeReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="BaseTypeReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public sealed override bool Equals(MemberInfo memberInfo)
            => Equals(memberInfo as Type);

        /// <summary>Determines whether the current <see cref="BaseTypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="BaseTypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public abstract bool Equals(Type type);

        internal abstract bool Equals(Type type, GenericMethodParameterReference originator, Type originatorMatch);
    }
}
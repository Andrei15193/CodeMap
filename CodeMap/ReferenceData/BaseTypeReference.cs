#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a base class for type reference such as concrete types, generic type definitions, arrays and so on.</summary>
    public abstract class BaseTypeReference : MemberReference, IEquatable<Type>
    {
        /// <summary>Determines whether the provided <paramref name="typeReference"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(BaseTypeReference typeReference, Type type)
            => Equals(typeReference, type);

        /// <summary>Determines whether the provided <paramref name="typeReference"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(BaseTypeReference typeReference, Type type)
            => !Equals(typeReference, type);

        /// <summary>Determines whether the provided <paramref name="typeReference"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Type type, BaseTypeReference typeReference)
            => Equals(typeReference, type);

        /// <summary>Determines whether the provided <paramref name="typeReference"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Type type, BaseTypeReference typeReference)
            => !Equals(typeReference, type);

        internal BaseTypeReference()
        {
        }

        /// <summary>Determines whether the current <see cref="BaseTypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="BaseTypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public abstract bool Equals(Type type);

        /// <summary>Determines whether the current <see cref="BaseTypeReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberInfo"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public sealed override bool Equals(MemberInfo memberInfo)
            => memberInfo is Type type && Equals(type);
    }
}
using System;
using System.Dynamic;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a type reference for <c>dynamic</c> types.</summary>
    public sealed class DynamicTypeReference : TypeReference, IEquatable<Type>
    {
        internal DynamicTypeReference()
        {
        }

        /// <summary>Determines whether the current <see cref="DynamicTypeReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DynamicTypeReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => memberInfo is Type type && Equals(type);

        /// <summary>Determines whether the current <see cref="DynamicTypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DynamicTypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public bool Equals(Type type)
            => type != null && (type == typeof(object) || typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type));
    }
}
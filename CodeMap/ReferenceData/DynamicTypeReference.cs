using System;
using System.Dynamic;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a type reference for <c>dynamic</c> types.</summary>
    public sealed class DynamicTypeReference : TypeReference
    {
        internal DynamicTypeReference()
        {
        }

        /// <summary>Determines whether the current <see cref="DynamicTypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DynamicTypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => type == typeof(object) || typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
    }
}
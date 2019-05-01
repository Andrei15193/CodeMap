using System;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a type reference for <c>void</c>.</summary>
    public sealed class VoidTypeReference : TypeReference
    {
        internal VoidTypeReference()
        {
        }

        /// <summary>Determines whether the current <see cref="VoidTypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="VoidTypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => type == typeof(void);
    }
}
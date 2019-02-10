using System;

namespace CodeMap.Elements
{
    /// <summary>Represents the type reference for <c>void</c>.</summary>
    public sealed class VoidTypeData : TypeReferenceData
    {
        /// <summary>Initializes a new instance of the <see cref="VoidTypeData"/></summary>
        public VoidTypeData()
        {
        }

        /// <summary>Determines whether the current <see cref="VoidTypeData"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="VoidTypeData"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => type == typeof(void);
    }
}
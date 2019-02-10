using System;

namespace CodeMap.Elements
{
    /// <summary>Represents a <c>dynamic</c> type reference.</summary>
    public sealed class DynamicTypeData : TypeReferenceData
    {
        /// <summary>Initializes a new instance of the <see cref="DynamicTypeData"/> class.</summary>
        public DynamicTypeData()
        {
        }

        /// <summary>Determines whether the current <see cref="DynamicTypeData"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DynamicTypeData"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => type == typeof(object);
    }
}
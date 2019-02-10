using System;

namespace CodeMap.Elements
{
    /// <summary>Represents a reference to an array type.</summary>
    public class ArrayTypeData : TypeReferenceData
    {
        internal ArrayTypeData()
        {
        }

        /// <summary>The rank of the array.</summary>
        public int Rank { get; internal set; }

        /// <summary>The item type within the array.</summary>
        public TypeReferenceData ItemType { get; internal set; }

        /// <summary>Determines whether the current <see cref="ArrayTypeData"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ArrayTypeData"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
        {
            if (type == null || !type.IsArray)
                return false;

            return type.GetArrayRank() == Rank && type.GetElementType() == ItemType;
        }
    }
}
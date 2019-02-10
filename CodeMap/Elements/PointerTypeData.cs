using System;

namespace CodeMap.Elements
{
    /// <summary>Represents a reference to a pointer type.</summary>
    public class PointerTypeData : TypeReferenceData
    {
        internal PointerTypeData()
        {
        }

        /// <summary>The type of the pointer.</summary>
        public TypeReferenceData ReferentType { get; internal set; }

        /// <summary>Determines whether the current <see cref="PointerTypeData"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PointerTypeData"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
        {
            if (type == null || !type.IsPointer)
                return false;

            return ReferentType == type.GetElementType();
        }
    }
}
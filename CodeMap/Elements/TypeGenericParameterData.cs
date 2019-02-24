﻿using System;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented type generic parameter.</summary>
    public sealed class TypeGenericParameterData : GenericParameterData
    {
        internal TypeGenericParameterData()
        {
        }

        /// <summary>The type declaring the generic parameter.</summary>
        public TypeDocumentationElement DeclaringType { get; internal set; }

        /// <summary>Determines whether the current <see cref="TypeGenericParameterData"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeGenericParameterData"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
        {
            if (type == null || !type.IsGenericTypeParameter)
                return false;

            return string.Equals(Name, type.Name, StringComparison.OrdinalIgnoreCase)
                && Position == type.GenericParameterPosition
                && DeclaringType == type.DeclaringType;
        }
    }
}
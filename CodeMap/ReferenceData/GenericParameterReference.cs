using System;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a generic parameter reference.</summary>
    public abstract class GenericParameterReference : BaseTypeReference
    {
        internal GenericParameterReference()
        {
        }

        /// <summary>The generic parameter name.</summary>
        public string Name { get; internal set; }

        /// <summary>Determines whether the current <see cref="GenericParameterReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="GenericParameterReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => type != null
            && type.IsGenericParameter
            && string.Equals(Name, type.Name, StringComparison.OrdinalIgnoreCase);
    }
}
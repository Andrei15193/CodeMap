#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Collections.Generic;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented generic parameter.</summary>
    public abstract class GenericParameterData : IEquatable<Type>
    {
        /// <summary>Determines whether the provided <paramref name="genericParameter"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="genericParameter">The <see cref="GenericParameterData"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(GenericParameterData genericParameter, Type type)
            => Equals(genericParameter, type);

        /// <summary>Determines whether the provided <paramref name="genericParameter"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="genericParameter">The <see cref="GenericParameterData"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(GenericParameterData genericParameter, Type type)
            => !Equals(genericParameter, type);

        /// <summary>Determines whether the provided <paramref name="genericParameter"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="genericParameter">The <see cref="GenericParameterData"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Type type, GenericParameterData genericParameter)
            => Equals(genericParameter, type);

        /// <summary>Determines whether the provided <paramref name="genericParameter"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="genericParameter">The <see cref="GenericParameterData"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Type type, GenericParameterData genericParameter)
            => !Equals(genericParameter, type);

        internal GenericParameterData()
        {
        }

        /// <summary>The name of the generic parameter.</summary>
        public string Name { get; internal set; }

        /// <summary>The position of the generic parameter.</summary>
        public int Position { get; internal set; }

        /// <summary>Indicates whether the parameter is covariant.</summary>
        public bool IsCovariant { get; internal set; }

        /// <summary>Indicates whether the parameter is contravariant.</summary>
        public bool IsContravariant { get; internal set; }

        /// <summary>Indicates whether the generic argument must be a reference type.</summary>
        public bool HasReferenceTypeConstraint { get; internal set; }

        /// <summary>Indicates whether the generic argument must be an unmanaged type.</summary>
        public bool HasUnmanagedTypeConstraint { get; internal set; }

        /// <summary>Indicates whether the generic argument must be a non nullable value type.</summary>
        public bool HasNonNullableValueTypeConstraint { get; internal set; }

        /// <summary>Indicates whether the generic argument must have a public parameterless constructor.</summary>
        public bool HasDefaultConstructorConstraint { get; internal set; }

        /// <summary>The generic argument type constraints (base class, implemented interfaces, generic argument inheritance).</summary>
        public IReadOnlyCollection<BaseTypeReference> TypeConstraints { get; internal set; }

        /// <summary>The generic parameter description.</summary>
        public BlockDescriptionDocumentationElement Description { get; internal set; }

        /// <summary>Determines whether the current <see cref="GenericParameterData"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="GenericParameterData"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public abstract bool Equals(Type type);

        /// <summary>Determines whether the current <see cref="GenericParameterData"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="GenericParameterData"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="Type"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="Type"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (obj is Type type)
                return Equals(type);
            else
                return base.Equals(obj);
        }
    }
}
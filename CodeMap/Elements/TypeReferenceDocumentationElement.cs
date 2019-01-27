#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;

namespace CodeMap.Elements
{
    /// <summary>Represents a type reference.</summary>
    public abstract class TypeReferenceDocumentationElement : DocumentationElement, IEquatable<Type>
    {
        /// <summary>Determines whether the provided <paramref name="typeReference"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="typeReference">The <see cref="TypeReferenceDocumentationElement"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(TypeReferenceDocumentationElement typeReference, Type type)
            => Equals(typeReference, type);

        /// <summary>Determines whether the provided <paramref name="typeReference"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="typeReference">The <see cref="TypeReferenceDocumentationElement"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(TypeReferenceDocumentationElement typeReference, Type type)
            => !Equals(typeReference, type);

        /// <summary>Determines whether the provided <paramref name="typeReference"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="typeReference">The <see cref="TypeReferenceDocumentationElement"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Type type, TypeReferenceDocumentationElement typeReference)
            => Equals(typeReference, type);

        /// <summary>Determines whether the provided <paramref name="typeReference"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="typeReference">The <see cref="TypeReferenceDocumentationElement"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Type type, TypeReferenceDocumentationElement typeReference)
            => !Equals(typeReference, type);

        internal TypeReferenceDocumentationElement()
        {
        }

        /// <summary>Determines whether the current <see cref="TypeReferenceDocumentationElement"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeReferenceDocumentationElement"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public abstract bool Equals(Type type);

        /// <summary>Determines whether the current <see cref="TypeReferenceDocumentationElement"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeReferenceDocumentationElement"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
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
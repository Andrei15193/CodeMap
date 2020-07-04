﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented property declared by a type.</summary>
    public sealed class PropertyDeclaration : MemberDeclaration, IEquatable<PropertyReference>
    {
        internal PropertyDeclaration()
        {
        }

        /// <summary>The property type.</summary>
        public BaseTypeReference Type { get; internal set; }

        /// <summary>Indicates whether the property is static.</summary>
        public bool IsStatic { get; internal set; }

        /// <summary>Indicates whether the property has been marked as virtual.</summary>
        public bool IsVirtual { get; internal set; }

        /// <summary>Indicates whether the property has been marked as abstract.</summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>Indicates whether the property is an override.</summary>
        public bool IsOverride { get; internal set; }

        /// <summary>Indicates whether the property has been marked as sealed.</summary>
        public bool IsSealed { get; internal set; }

        /// <summary>Indicates whether the property hides a member from a base type.</summary>
        public bool IsShadowing { get; internal set; }

        /// <summary>Information about the getter accessor.</summary>
        public PropertyAccessorData Getter { get; internal set; }

        /// <summary>Information about the setter accessor.</summary>
        public PropertyAccessorData Setter { get; internal set; }

        /// <summary>The method parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>Documentation about the how the value of the property is calculated.</summary>
        public ValueDocumentationElement Value { get; internal set; }

        /// <summary>Documented exceptions that might be thrown when using the property.</summary>
        public IReadOnlyCollection<ExceptionData> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitProperty(this);

        /// <summary>Determines whether the current <see cref="PropertyDeclaration"/> is equal to the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyDeclaration"/> references the provided <paramref name="memberReference"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberReference memberReference)
            => memberReference is PropertyReference propertyReference
            && Equals(propertyReference);

        /// <summary>Determines whether the current <see cref="PropertyDeclaration"/> is equal to the provided <paramref name="propertyReference"/>.</summary>
        /// <param name="propertyReference">The <see cref="PropertyReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyDeclaration"/> references the provided <paramref name="propertyReference"/>; <c>false</c> otherwise.</returns>
        public bool Equals(PropertyReference propertyReference)
            => propertyReference != null
            && string.Equals(Name, propertyReference.Name, StringComparison.OrdinalIgnoreCase)
            && DeclaringType == propertyReference.DeclaringType
            && Parameters.Select(parameter => parameter.Type).SequenceEqual(propertyReference.ParameterTypes);
    }
}
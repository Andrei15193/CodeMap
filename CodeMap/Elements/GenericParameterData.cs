using System.Collections.Generic;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented generic parameter.</summary>
    public abstract class GenericParameterData : TypeReferenceData
    {
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

        /// <summary>Indicates whether the generic argument must be a non nullable value type.</summary>
        public bool HasNonNullableValueTypeConstraint { get; internal set; }

        /// <summary>Indicates whether the generic argument must have a public parameterless constructor.</summary>
        public bool HasDefaultConstructorConstraint { get; internal set; }

        /// <summary>The generic argument type constraints (base class, implemented interfaces, generic argument inheritance).</summary>
        public IReadOnlyCollection<TypeReferenceData> TypeConstraints { get; internal set; }

        /// <summary>The generic parameter description.</summary>
        public DescriptionDocumentationElement Description { get; internal set; }
    }
}
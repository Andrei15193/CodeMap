﻿using System.Collections.Generic;
using System.Linq;

namespace CodeMap
{
    /// <summary>Contains the information used to decorate a member with an attribute.</summary>
    public class AttributeData
    {
        /// <summary>Initializes a new instance of the <see cref="AttributeData"/> class.</summary>
        /// <param name="type">The type of the attribute.</param>
        /// <param name="positionalParameters">The used positional (constructor) parameters.</param>
        /// <param name="namedParameters">The used named parameters (fields or properties).</param>
        public AttributeData(TypeReferenceData type, IEnumerable<AttributeParameterData> positionalParameters, IEnumerable<AttributeParameterData> namedParameters)
        {
            Type = type;
            PositionalParameters = positionalParameters as IReadOnlyList<AttributeParameterData> ?? positionalParameters.ToList();
            NamedParameters = namedParameters as IReadOnlyList<AttributeParameterData> ?? namedParameters.ToList();
        }

        /// <summary>The type of the attribute.</summary>
        public TypeReferenceData Type { get; set; }

        /// <summary>The used positional (constructor) parameters.</summary>
        public IReadOnlyList<AttributeParameterData> PositionalParameters { get; set; }

        /// <summary>The used named parameters (fields or properties).</summary>
        public IReadOnlyList<AttributeParameterData> NamedParameters { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Contains the information used to decorate a member with an attribute.</summary>
    public class AttributeData
    {
        /// <summary>Initializes a new instance of the <see cref="AttributeData"/> class.</summary>
        /// <param name="type">The type of the attribute.</param>
        /// <param name="positionalParameters">The used positional (constructor) parameters.</param>
        /// <param name="namedParameters">The used named parameters (fields or properties).</param>
        public AttributeData(TypeReference type, IEnumerable<AttributeParameterData> positionalParameters, IEnumerable<AttributeParameterData> namedParameters)
        {
            Type = type;

            PositionalParameters = positionalParameters.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(positionalParameters));
            if (PositionalParameters.Contains(null))
                throw new ArgumentException("Cannot contain 'null' parameters.", nameof(positionalParameters));

            NamedParameters = namedParameters.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(namedParameters));
            if (NamedParameters.Contains(null))
                throw new ArgumentException("Cannot contain 'null' parameters.", nameof(namedParameters));
        }

        /// <summary>The type of the attribute.</summary>
        public TypeReference Type { get; }

        /// <summary>The used positional (constructor) parameters.</summary>
        public IReadOnlyList<AttributeParameterData> PositionalParameters { get; }

        /// <summary>The used named parameters (fields or properties).</summary>
        public IReadOnlyList<AttributeParameterData> NamedParameters { get; }
    }
}
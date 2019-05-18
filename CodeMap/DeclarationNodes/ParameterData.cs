using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;
using System.Collections.Generic;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented parameter.</summary>
    public class ParameterData
    {
        internal ParameterData()
        {
        }

        /// <summary>The parameter name.</summary>
        public string Name { get; internal set; }

        /// <summary>The parameter type.</summary>
        public BaseTypeReference Type { get; internal set; }

        /// <summary>The parameter attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>Indicates whether the parameter passed by reference and is input only (decorated with <c>in</c> in C#).</summary>
        public bool IsInputByReference { get; internal set; }

        /// <summary>Indicates whether the parameter passed by reference and is input and output (decorated with <c>ref</c> in C#).</summary>
        public bool IsInputOutputByReference { get; internal set; }

        /// <summary>Indicates whether the parameter passed by reference and is output only (decorated with <c>out</c> in C#).</summary>
        public bool IsOutputByReference { get; internal set; }

        /// <summary>Indicates whether the parameter has a default value.</summary>
        public bool HasDefaultValue { get; internal set; }

        /// <summary>The parameter default value</summary>
        /// <remarks>
        /// This property must be used in conjunction with <see cref="HasDefaultValue"/> as <c>null</c> can be a valid default value
        /// and therefore cannot be used to determine whether there is a default value.
        /// </remarks>
        public object DefaultValue { get; internal set; }

        /// <summary>The parameter description.</summary>
        public BlockDescriptionDocumentationElement Description { get; internal set; }
    }
}
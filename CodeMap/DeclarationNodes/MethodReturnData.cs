using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;
using System.Collections.Generic;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented return type.</summary>
    public sealed class MethodReturnData
    {
        internal MethodReturnData()
        {
        }

        /// <summary>The return type.</summary>
        public BaseTypeReference Type { get; internal set; }

        /// <summary>The return attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The content of the returns section.</summary>
        public BlockDescriptionDocumentationElement Description { get; internal set; }
    }
}
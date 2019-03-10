using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented return type.</summary>
    public sealed class ReturnsData
    {
        internal ReturnsData()
        {
        }

        /// <summary>The return type.</summary>
        public TypeReferenceData Type { get; internal set; }

        /// <summary>The return attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The content of the returns section.</summary>
        public BlockDescriptionDocumentationElement Description { get; internal set; }
    }
}
using System.Collections.Generic;

namespace CodeMap.Elements
{
    /// <summary>Represents accessor information (getter and setter) for a property.</summary>
    public sealed class PropertyAccessorData
    {
        internal PropertyAccessorData()
        {
        }

        /// <summary>The property accessor modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The accessor attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The accessor return attributes.</summary>
        public IReadOnlyCollection<AttributeData> ReturnAttributes { get; internal set; }
    }
}
using System.Collections.Generic;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents getter information of a property.</summary>
    public class PropertyGetterData
    {
        internal PropertyGetterData()
        {
        }

        /// <summary>The property getter access modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The getter attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The getter return attributes.</summary>
        public IReadOnlyCollection<AttributeData> ReturnAttributes { get; internal set; }
    }
}
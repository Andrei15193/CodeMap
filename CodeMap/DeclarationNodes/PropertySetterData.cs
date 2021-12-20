using System.Collections.Generic;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents setter information of a property.</summary>
    public class PropertySetterData
    {
        internal PropertySetterData()
        {
        }

        /// <summary>Indicates whether the property can be set only when an instance is being initialized.</summary>
        public bool IsInitOnly { get; internal set; }

        /// <summary>The property setter access modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The setter attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The setter return attributes.</summary>
        public IReadOnlyCollection<AttributeData> ReturnAttributes { get; internal set; }
    }
}
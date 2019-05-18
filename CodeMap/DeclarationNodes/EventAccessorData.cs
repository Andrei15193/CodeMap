using System.Collections.Generic;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents accessor information (adder and remover) for an event.</summary>
    public sealed class EventAccessorData
    {
        internal EventAccessorData()
        {
        }

        /// <summary>The accessor attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The accessor return attributes.</summary>
        public IReadOnlyCollection<AttributeData> ReturnAttributes { get; internal set; }
    }
}
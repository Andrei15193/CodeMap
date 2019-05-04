namespace CodeMap.ReferenceData
{
    /// <summary>Represents a generic parameter reference.</summary>
    public abstract class GenericParameterReference : BaseTypeReference
    {
        internal GenericParameterReference()
        {
        }

        /// <summary>The generic parameter name.</summary>
        public string Name { get; internal set; }
    }
}
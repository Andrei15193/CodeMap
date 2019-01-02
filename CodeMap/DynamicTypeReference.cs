namespace CodeMap
{
    /// <summary>Represents a <c>dynamic</c> type reference.</summary>
    public sealed class DynamicTypeReference : TypeReferenceData
    {
        /// <summary>Initializes a new instance of the <see cref="DynamicTypeReference"/> class.</summary>
        public DynamicTypeReference()
            :base("dynamic")
        {
        }
    }
}
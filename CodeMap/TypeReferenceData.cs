namespace CodeMap
{
    /// <summary>Represents a type reference.</summary>
    public abstract class TypeReferenceData
    {
        internal TypeReferenceData(string name)
        {
            Name = name;
        }

        /// <summary>The name of the type.</summary>
        public string Name { get; }
    }
}
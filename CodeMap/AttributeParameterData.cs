using CodeMap.ReferenceData;

namespace CodeMap
{
    /// <summary>Contains information used for an attribute parameter.</summary>
    public class AttributeParameterData
    {
        internal AttributeParameterData()
        {
        }

        /// <summary>The type of the parameter.</summary>
        public BaseTypeReference Type { get; internal set; }

        /// <summary>The name of the parameter.</summary>
        public string Name { get; internal set; }

        /// <summary>The value of the parameter.</summary>
        public object Value { get; internal set; }
    }
}
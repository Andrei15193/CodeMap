namespace CodeMap
{
    /// <summary>Contains information used for an attribute parameter.</summary>
    public class AttributeParameterData
    {
        /// <summary>Initializes a new instance of the <see cref="AttributeParameterData"/> class.</summary>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public AttributeParameterData(TypeReferenceData type, string name, object value)
        {
        }

        /// <summary>The type of the parameter.</summary>
        public TypeReferenceData Type { get;  }

        /// <summary>The name of the parameter.</summary>
        public string Name { get;  }

        /// <summary>The value of the parameter.</summary>
        public object Value { get; }
    }
}
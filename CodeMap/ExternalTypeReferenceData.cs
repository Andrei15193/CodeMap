namespace CodeMap
{
    /// <summary>Represents an external (different assembly) type reference.</summary>
    public class ExternalTypeReferenceData : TypeReferenceData
    {
        /// <summary>Initializes a new instance of the <see cref="ExternalTypeReferenceData"/> class.</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="namespace">The namespace of the type.</param>
        /// <param name="declaringType">The declaring type, if any.</param>
        /// <param name="assembly">The declaring assembly.</param>
        public ExternalTypeReferenceData(string name, string @namespace, ExternalTypeReferenceData declaringType, AssemblyReferenceData assembly)
            : base(name)
        {
            Namespace = @namespace ?? string.Empty;
            DeclaringType = declaringType;
            Assembly = assembly;
        }

        /// <summary>The namespace of the type.</summary>
        public string Namespace { get;  }

        /// <summary>The declaring type, if any.</summary>
        public ExternalTypeReferenceData DeclaringType { get; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyReferenceData Assembly { get;  }
    }
}
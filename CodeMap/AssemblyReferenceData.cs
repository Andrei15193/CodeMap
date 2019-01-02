using System;

namespace CodeMap
{
    /// <summary>Contains all the necessary information for referencing a .NET assembly.</summary>
    public class AssemblyReferenceData
    {
        /// <summary>Initializes a new instance of the <see cref="AssemblyReferenceData"/> class.</summary>
        /// <param name="name">The name of the assembly.</param>
        /// <param name="version">The version of the assembly.</param>
        /// <param name="culture">The culture of the assembly, if it is a satelite one.</param>
        /// <param name="publicKeyToken">The public key token for the assembly, if signed.</param>
        public AssemblyReferenceData(string name, Version version, string culture, string publicKeyToken)
        {
            Name = name;
            Version = version;
            Culture = culture ?? string.Empty;
            PublicKeyToken = publicKeyToken ?? string.Empty;
        }

        /// <summary>The name of the assembly.</summary>
        public string Name { get; }

        /// <summary>The version of the assembly.</summary>
        public Version Version { get; }

        /// <summary>The culture of the assembly if it is a satelite one; otherwise <see cref="string.Empty"/>.</summary>
        public string Culture { get; }

        /// <summary>The public key token if the assembly is signed; otherwise <see cref="string.Empty"/>.</summary>
        public string PublicKeyToken { get; }
    }
}
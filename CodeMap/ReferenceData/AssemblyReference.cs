#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a documented .NET assembly reference.</summary>
    public sealed class AssemblyReference : IEquatable<Assembly>, IEquatable<AssemblyName>
    {
        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assemblyName"/> are equal.</summary>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyReference assemblyReference, AssemblyName assemblyName)
            => Equals(assemblyReference, assemblyName);

        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assemblyName"/> are not equal.</summary>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(AssemblyReference assemblyReference, AssemblyName assemblyName)
            => !Equals(assemblyReference, assemblyName);

        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assemblyName"/> are equal.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyName assemblyName, AssemblyReference assemblyReference)
            => Equals(assemblyReference, assemblyName);

        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assemblyName"/> are not equal.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(AssemblyName assemblyName, AssemblyReference assemblyReference)
            => !Equals(assemblyReference, assemblyName);

        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assembly"/> are equal.</summary>
        /// <param name="assemblyReference">The <see cref="Assembly"/> to compare.</param>
        /// <param name="assembly">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyReference assemblyReference, Assembly assembly)
            => Equals(assemblyReference, assembly);

        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assembly"/> are not equal.</summary>
        /// <param name="assemblyReference">The <see cref="Assembly"/> to compare.</param>
        /// <param name="assembly">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(AssemblyReference assemblyReference, Assembly assembly)
            => !Equals(assemblyReference, assembly);

        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assembly"/> are equal.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Assembly assembly, AssemblyReference assemblyReference)
            => Equals(assemblyReference, assembly);

        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assembly"/> are not equal.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Assembly assembly, AssemblyReference assemblyReference)
            => !Equals(assemblyReference, assembly);

        internal AssemblyReference()
        {
        }

        /// <summary>The assembly name.</summary>
        public string Name { get; internal set; }

        /// <summary>The assembly version.</summary>
        public Version Version { get; internal set; }

        /// <summary>The assembly culture if it is a satelite one; otherwise <see cref="string.Empty"/>.</summary>
        public string Culture { get; internal set; }

        /// <summary>The assembly public key token if it is signed; otherwise <see cref="string.Empty"/>.</summary>
        public string PublicKeyToken { get; internal set; }

        /// <summary>Determines whether the current <see cref="AssemblyReference"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="AssemblyReference"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        public bool Equals(AssemblyName assemblyName)
            => assemblyName != null
                && string.Equals(Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase)
                && Version == assemblyName.Version
                && string.Equals(Culture, assemblyName.CultureName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(PublicKeyToken, assemblyName.GetPublicKeyToken().ToBase16String(), StringComparison.OrdinalIgnoreCase);

        /// <summary>Determines whether the current <see cref="AssemblyReference"/> is equal to the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="AssemblyReference"/> references the provided <paramref name="assembly"/>; <c>false</c> otherwise.</returns>
        public bool Equals(Assembly assembly)
            => Equals(assembly?.GetName());

        /// <summary>Determines whether the current <see cref="AssemblyReference"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="AssemblyReference"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is an <see cref="AssemblyName"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="AssemblyName"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (obj is AssemblyName assemblyName)
                return Equals(assemblyName);
            else if (obj is Assembly assembly)
                return Equals(assembly);
            else
                return base.Equals(obj);
        }
    }
}
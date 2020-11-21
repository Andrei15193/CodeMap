#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a documented .NET assembly reference.</summary>
    public sealed class AssemblyReference : MemberReference
    {
        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assembly"/> are equal.</summary>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyReference assemblyReference, Assembly assembly)
            => Equals(assemblyReference, assembly);

        /// <summary>Determines whether the provided <paramref name="assemblyReference"/> and <paramref name="assembly"/> are not equal.</summary>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
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

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitAssembly(this);

        /// <summary>Determines whether the current <see cref="MemberReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        /// <remarks>This method always returns <c>false</c> because a <see cref="MemberInfo"/> cannot represent an assembly.</remarks>
        public override bool Equals(MemberInfo memberInfo)
            => false;

        /// <summary>Determines whether the current <see cref="AssemblyReference"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="AssemblyReference"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(AssemblyName assemblyName)
            => assemblyName != null
                && string.Equals(Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase)
                && Version == assemblyName.Version
                && string.Equals(Culture, assemblyName.CultureName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(PublicKeyToken, assemblyName.GetPublicKeyToken().ToBase16String(), StringComparison.OrdinalIgnoreCase);
    }
}
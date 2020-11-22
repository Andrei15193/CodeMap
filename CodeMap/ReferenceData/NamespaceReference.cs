using System;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a namespace reference.</summary>
    public class NamespaceReference : MemberReference
    {
        internal NamespaceReference(string name, AssemblyReference declaringAssembly)
            => (Name, Assembly) = (name, declaringAssembly);

        /// <summary>The namespace name, or <see cref="string.Empty"/> for the global namespace.</summary>
        public string Name { get; }

        /// <summary>The declaring assembly.</summary>
        public override AssemblyReference Assembly { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitNamespace(this);

        /// <summary>Determines whether the current <see cref="NamespaceReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="NamespaceReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        /// <remarks>This method always returns <c>false</c> because a <see cref="MemberInfo"/> cannot represent a namespace reference.</remarks>
        public override bool Equals(MemberInfo memberInfo)
            => false;

        /// <summary>Determines whether the current <see cref="NamespaceReference"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="NamespaceReference"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        /// <remarks>This method always returns <c>false</c> because an <see cref="AssemblyName"/> cannot represent a namespace reference.</remarks>
        public sealed override bool Equals(AssemblyName assemblyName)
            => false;
    }
}
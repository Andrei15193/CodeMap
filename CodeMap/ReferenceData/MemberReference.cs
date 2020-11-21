#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents an Assembly member reference.</summary>
    public abstract class MemberReference : IEquatable<MemberInfo>, IEquatable<Assembly>, IEquatable<AssemblyName>
    {
        /// <summary>Determines whether the provided <paramref name="memberReference"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(MemberReference memberReference, MemberInfo memberInfo)
            => Equals(memberReference, memberInfo);

        /// <summary>Determines whether the provided <paramref name="memberReference"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(MemberInfo memberInfo, MemberReference memberReference)
            => Equals(memberReference, memberInfo);

        /// <summary>Determines whether the provided <paramref name="memberReference"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(MemberReference memberReference, MemberInfo memberInfo)
            => !Equals(memberReference, memberInfo);

        /// <summary>Determines whether the provided <paramref name="memberReference"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(MemberInfo memberInfo, MemberReference memberReference)
            => !Equals(memberReference, memberInfo);

        internal MemberReference()
        {
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public abstract void Accept(MemberReferenceVisitor visitor);

        /// <summary>Determines whether the current <see cref="MemberReference"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberReference"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="MemberInfo"/>, <see cref="Assembly"/> or <see cref="AssemblyName"/> then the
        /// comparison is done by checking whether the current instance actually maps to the provided actual instance. Otherwise the equality is
        /// determined by comparing references.
        /// </remarks>
        public sealed override bool Equals(object obj)
            => obj switch
            {
                MemberInfo memberInfo => Equals(memberInfo),
                Assembly assembly => Equals(assembly),
                AssemblyName assemblyName => Equals(assemblyName),
                _ => base.Equals(obj)
            };

        /// <summary>Determines whether the current <see cref="MemberReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public abstract bool Equals(MemberInfo memberInfo);

        /// <summary>Determines whether the current <see cref="MemberReference"/> is equal to the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberReference"/> references the provided <paramref name="assembly"/>; <c>false</c> otherwise.</returns>
        public bool Equals(Assembly assembly)
            => assembly != null && Equals(assembly.GetName());

        /// <summary>Determines whether the current <see cref="MemberReference"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberReference"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        public abstract bool Equals(AssemblyName assemblyName);
    }
}
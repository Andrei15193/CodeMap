using System;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a reference to a field.</summary>
    public class FieldReference : MemberReference, IEquatable<FieldInfo>
    {
        internal FieldReference()
        {
        }

        /// <summary>The field name.</summary>
        public string Name { get; internal set; }

        /// <summary>The field declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitField(this);

        /// <summary>Determines whether the current <see cref="FieldReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="FieldReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => Equals(memberInfo as FieldInfo);

        /// <summary>Determines whether the current <see cref="FieldReference"/> is equal to the provided <paramref name="fieldInfo"/>.</summary>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="FieldReference"/> references the provided <paramref name="fieldInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(FieldInfo fieldInfo)
            => fieldInfo != null
               && Name.Equals(fieldInfo.Name, StringComparison.OrdinalIgnoreCase)
               && DeclaringType.Equals(fieldInfo.DeclaringType);

        /// <summary>Determines whether the current <see cref="FieldReference"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="FieldReference"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        /// <remarks>This method always returns <c>false</c> because an <see cref="AssemblyName"/> cannot represent a field reference.</remarks>
        public sealed override bool Equals(AssemblyName assemblyName)
            => false;
    }
}
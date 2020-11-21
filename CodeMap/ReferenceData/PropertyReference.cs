using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a property reference.</summary>
    public sealed class PropertyReference : MemberReference
    {
        internal PropertyReference()
        {
        }

        /// <summary>The property name.</summary>
        public string Name { get; internal set; }

        /// <summary>The property declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The property parameter types.</summary>
        public IReadOnlyList<BaseTypeReference> ParameterTypes { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitProperty(this);

        /// <summary>Determines whether the current <see cref="PropertyReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => Equals(memberInfo as PropertyInfo);

        /// <summary>Determines whether the current <see cref="PropertyReference"/> is equal to the provided <paramref name="propertyInfo"/>.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyReference"/> references the provided <paramref name="propertyInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(PropertyInfo propertyInfo)
            => propertyInfo != null
               && propertyInfo.DeclaringType == DeclaringType
               && string.Equals(propertyInfo.Name, Name, StringComparison.OrdinalIgnoreCase)
               && ParameterTypes.Count == propertyInfo.GetIndexParameters().Length
               && ParameterTypes
                   .Zip(propertyInfo.GetIndexParameters(), (parameterType, parameter) => (Expected: parameterType, Actual: parameter.ParameterType))
                   .All(pair => pair.Expected.Equals(pair.Actual));

        /// <summary>Determines whether the current <see cref="PropertyReference"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyReference"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        /// <remarks>This method always returns <c>false</c> because an <see cref="AssemblyName"/> cannot represent a property reference.</remarks>
        public sealed override bool Equals(AssemblyName assemblyName)
            => false;
    }
}
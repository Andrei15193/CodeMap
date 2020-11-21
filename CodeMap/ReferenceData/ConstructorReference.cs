using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a constructor reference.</summary>
    public sealed class ConstructorReference : MemberReference, IEquatable<ConstructorInfo>
    {
        internal ConstructorReference()
        {
        }

        /// <summary>The constructor declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The constructor parameter types.</summary>
        public IReadOnlyList<BaseTypeReference> ParameterTypes { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitConstructor(this);

        /// <summary>Determines whether the current <see cref="ConstructorReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstructorReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => Equals(memberInfo as ConstructorInfo);

        /// <summary>Determines whether the current <see cref="ConstructorReference"/> is equal to the provided <paramref name="constructorInfo"/>.</summary>
        /// <param name="constructorInfo">The <see cref="ConstructorInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstructorReference"/> references the provided <paramref name="constructorInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(ConstructorInfo constructorInfo)
            => constructorInfo != null
               && DeclaringType.Equals(constructorInfo.DeclaringType)
               && ParameterTypes.Count == constructorInfo.GetParameters().Length
               && ParameterTypes
                    .Zip(constructorInfo.GetParameters(), (parameterType, parameter) => (Expected: parameterType, Actual: parameter.ParameterType))
                    .All(pair => pair.Expected.Equals(pair.Actual));

        /// <summary>Determines whether the current <see cref="ConstructorReference"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="BaseTypeReference"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        /// <remarks>This method always returns <c>false</c> because an <see cref="ConstructorReference"/> cannot represent a constructor reference.</remarks>
        public sealed override bool Equals(AssemblyName assemblyName)
            => false;
    }
}
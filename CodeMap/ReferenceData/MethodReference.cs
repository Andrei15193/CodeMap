using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a method reference.</summary>
    public sealed class MethodReference : MemberReference, IEquatable<MethodInfo>
    {
        internal MethodReference()
        {
        }

        /// <summary>The method name.</summary>
        public string Name { get; internal set; }

        /// <summary>The method generic arguments. These can be generic parameter declarations or actual types in case of a constructed generic method.</summary>
        public IReadOnlyCollection<BaseTypeReference> GenericArguments { get; internal set; }

        /// <summary>The method declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The method parameter types.</summary>
        public IReadOnlyList<BaseTypeReference> ParameterTypes { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public override AssemblyReference Assembly
            => DeclaringType.Assembly;

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitMethod(this);

        /// <summary>Determines whether the current <see cref="MethodReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MethodReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => Equals(memberInfo as MethodInfo);

        /// <summary>Determines whether the current <see cref="MethodReference"/> is equal to the provided <paramref name="methodInfo"/>.</summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MethodReference"/> references the provided <paramref name="methodInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(MethodInfo methodInfo)
            => Equals(methodInfo, null, null);

        /// <summary>Determines whether the current <see cref="MethodReference"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MethodReference"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        /// <remarks>This method always returns <c>false</c> because an <see cref="AssemblyName"/> cannot represent a method reference.</remarks>
        public sealed override bool Equals(AssemblyName assemblyName)
            => false;

        internal bool Equals(MethodBase methodBase, GenericMethodParameterReference originator, Type originatorMatch)
            => methodBase != null
               && DeclaringType.Equals(methodBase.DeclaringType)
               && Name.Equals(methodBase.Name, StringComparison.OrdinalIgnoreCase)
               && methodBase.GetGenericArguments().Length == GenericArguments.Count
               && (
                   methodBase.IsConstructedGenericMethod
                       ? GenericArguments
                           .Zip(methodBase.GetGenericArguments(), (expectedGenericArgument, actualGenericArgument) => (ExpectedGenericArgument: expectedGenericArgument, ActualGenericArgument: actualGenericArgument))
                           .All(pair => pair.ExpectedGenericArgument.Equals(pair.ActualGenericArgument, originator, originatorMatch))
                       : GenericArguments
                           .All(genericArgument => genericArgument is GenericMethodParameterReference genericMethodParameter && ReferenceEquals(this, genericMethodParameter.DeclaringMethod))
               )
               && ParameterTypes.Count == methodBase.GetParameters().Length
               && ParameterTypes
                   .Zip(methodBase.GetParameters(), (parameterType, parameter) => (Expected: parameterType, Actual: parameter.ParameterType))
                   .All(pair => pair.Expected.Equals(pair.Actual, originator, originatorMatch));
    }
}
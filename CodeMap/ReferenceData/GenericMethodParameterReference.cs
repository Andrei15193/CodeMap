using System;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a generic method parameter reference.</summary>
    public sealed class GenericMethodParameterReference : GenericParameterReference
    {
        internal GenericMethodParameterReference()
        {
        }

        /// <summary>The generic parameter declaring method.</summary>
        public MethodReference DeclaringMethod { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public override AssemblyReference Assembly
            => DeclaringMethod.Assembly;

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitGenericMethodParameter(this);

        /// <summary>Determines whether the current <see cref="GenericMethodParameterReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="GenericMethodParameterReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => type is object
               && type.IsGenericMethodParameter
               && Position == type.GenericParameterPosition
               && DeclaringMethod.Equals(type.DeclaringMethod, this, type);

        internal override bool Equals(Type type, GenericMethodParameterReference originator, Type originatorMatch)
            => originator is null && originatorMatch is null ? Equals(type) : ReferenceEquals(this, originator) && ReferenceEquals(type, originatorMatch);
    }
}
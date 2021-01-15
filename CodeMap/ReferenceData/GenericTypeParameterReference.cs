using System;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a generic type parameter reference.</summary>
    public sealed class GenericTypeParameterReference : GenericParameterReference
    {
        internal GenericTypeParameterReference()
        {
        }

        /// <summary>The generic parameter declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public override AssemblyReference Assembly
            => DeclaringType.Assembly;

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitGenericTypeParameter(this);

        /// <summary>Determines whether the current <see cref="GenericMethodParameterReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="GenericMethodParameterReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => Equals(type, null, null);

        internal override bool Equals(Type type, GenericMethodParameterReference originator, Type originatorMatch)
            => type is object
               && type.IsGenericTypeParameter
               && Position == type.GenericParameterPosition
               && DeclaringType.Equals(type.DeclaringType, originator, originatorMatch);
    }
}
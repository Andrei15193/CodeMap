using System;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a pointer type reference.</summary>
    public sealed class PointerTypeReference : BaseTypeReference
    {
        internal PointerTypeReference()
        {
        }

        /// <summary>The type of the pointer.</summary>
        public BaseTypeReference ReferentType { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitPointer(this);

        /// <summary>Determines whether the current <see cref="PointerTypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PointerTypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => Equals(type, null, null);

        internal override bool Equals(Type type, GenericMethodParameterReference originator, Type originatorMatch)
            => type != null && type.IsPointer && ReferentType.Equals(type.GetElementType(), originator, originatorMatch);
    }
}
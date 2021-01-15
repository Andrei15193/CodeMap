using System;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents an array type reference.</summary>
    public sealed class ArrayTypeReference : BaseTypeReference
    {
        internal ArrayTypeReference()
        {
        }

        /// <summary>The array rank.</summary>
        public int Rank { get; internal set; }

        /// <summary>The array item type.</summary>
        public BaseTypeReference ItemType { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public override AssemblyReference Assembly
            => ItemType.Assembly;

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitArray(this);

        /// <summary>Determines whether the current <see cref="ArrayTypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ArrayTypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => Equals(type, null, null);

        internal override bool Equals(Type type, GenericMethodParameterReference originator, Type originatorMatch)
            => type is object && type.IsArray && Rank == type.GetArrayRank() && ItemType.Equals(type.GetElementType(), originator, originatorMatch);
    }
}
using System;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents the type of a value passed by ref.</summary>
    public sealed class ByRefTypeReference : BaseTypeReference
    {
        internal ByRefTypeReference()
        {
        }

        /// <summary>The value type passed by ref.</summary>
        public BaseTypeReference ReferentType { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public override AssemblyReference Assembly
            => ReferentType.Assembly;

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitByRef(this);

        /// <summary>Determines whether the current <see cref="ByRefTypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ByRefTypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => Equals(type, null, null);

        internal override bool Equals(Type type, GenericMethodParameterReference originator, Type originatorMatch)
            => type is object && type.IsByRef && ReferentType.Equals(type.GetElementType(), originator, originatorMatch);
    }
}
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

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitByRef(this);
    }
}
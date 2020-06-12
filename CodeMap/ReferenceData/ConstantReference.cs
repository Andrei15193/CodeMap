using System;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a reference to a constant field.</summary>
    public sealed class ConstantReference : FieldReference
    {
        internal ConstantReference()
        {
        }

        /// <summary>The constant value.</summary>
        public object Value { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitConstant(this);
    }
}
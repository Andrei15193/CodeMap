using System;
using System.Collections.Generic;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a constructor reference.</summary>
    public sealed class ConstructorReference : MemberReference
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
    }
}
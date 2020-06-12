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

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitGenericMethodParameter(this);
    }
}
using System;
using System.Collections.Generic;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a property reference.</summary>
    public sealed class PropertyReference : MemberReference
    {
        internal PropertyReference()
        {
        }

        /// <summary>The property name.</summary>
        public string Name { get; internal set; }

        /// <summary>The property declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The property parameter types.</summary>
        public IReadOnlyList<BaseTypeReference> ParameterTypes { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitProperty(this);
    }
}
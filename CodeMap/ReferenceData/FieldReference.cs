using System;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a reference to a field.</summary>
    public class FieldReference : MemberReference
    {
        internal FieldReference()
        {
        }

        /// <summary>The field name.</summary>
        public string Name { get; internal set; }

        /// <summary>The field declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitField(this);
    }
}
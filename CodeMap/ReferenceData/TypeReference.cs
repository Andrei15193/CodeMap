using System;
using System.Collections.Generic;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a type reference.</summary>
    public class TypeReference : BaseTypeReference
    {
        internal TypeReference()
        {
        }

        /// <summary>The type name.</summary>
        public string Name { get; internal set; }

        /// <summary>The type namespace.</summary>
        public string Namespace { get; internal set; }

        /// <summary>The type generic arguments. These can be generic parameter declarations or actual types in case of a constructed generic type.</summary>
        public IReadOnlyList<BaseTypeReference> GenericArguments { get; internal set; }

        /// <summary>The declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyReference Assembly { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitType(this);
    }
}
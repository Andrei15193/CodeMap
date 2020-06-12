using System;
using System.Collections.Generic;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a method reference.</summary>
    public sealed class MethodReference : MemberReference
    {
        internal MethodReference()
        {
        }

        /// <summary>The method name.</summary>
        public string Name { get; internal set; }

        /// <summary>The method generic arguments. These can be generic parameter declarations or actual types in case of a constructed generic method.</summary>
        public IReadOnlyCollection<BaseTypeReference> GenericArguments { get; internal set; }

        /// <summary>The method declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The method parameter types.</summary>
        public IReadOnlyList<BaseTypeReference> ParameterTypes { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitMethod(this);
    }
}
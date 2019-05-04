﻿#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override Task AcceptAsync(MemberReferenceVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitTypeAsync(this, cancellationToken);
    }
}
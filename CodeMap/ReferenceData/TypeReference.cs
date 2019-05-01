#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>The type generic arguments.</summary>
        public IReadOnlyList<BaseTypeReference> GenericArguments { get; internal set; }

        /// <summary>The declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyReference Assembly { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException(nameof(visitor));

            visitor.VisitType(this);
        }

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override Task AcceptAsync(MemberReferenceVisitor visitor, CancellationToken cancellationToken)
        {
            if (visitor == null)
                return Task.FromException(new ArgumentNullException(nameof(visitor)));

            return visitor.VisitTypeAsync(this, cancellationToken);
        }

        /// <summary>Determines whether the current <see cref="TypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
        {
            if (type == null || type.IsPointer || type.IsArray || type.IsByRef || type.IsGenericParameter)
                return false;

            var declaryingType = type.GetDeclaringType();
            var genericArgumentsOffset = declaryingType?.GetGenericArguments().Length ?? 0;
            var genericArguments = type.GetGenericArguments();
            return
                Name.AsSpan().Equals(type.GetTypeName(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(Namespace, type.Namespace, StringComparison.OrdinalIgnoreCase)
                && DeclaringType == declaryingType
                && GenericArguments.Count == genericArguments.Length - genericArgumentsOffset
                && _CompareGenericArguments(type, genericArguments.Skip(genericArgumentsOffset))
                && Assembly == type.Assembly.GetName();
        }

        private bool _CompareGenericArguments(Type type, IEnumerable<Type> genericArguments)
        {
            if (GenericArguments.OfType<GenericParameterReference>().Any())
                return type.IsGenericTypeDefinition;
            else
                return GenericArguments.Zip(
                    genericArguments,
                    (typeReference, genericArgument) => (TypeReference: typeReference, GenericArgument: genericArgument)
                )
                .All(pair => pair.TypeReference == pair.GenericArgument);
        }
    }
}
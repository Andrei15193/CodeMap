using System;
using System.Collections.Generic;
using System.Linq;

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
        public NamespaceReference Namespace { get; internal set; }

        /// <summary>The type generic arguments. These can be generic parameter declarations or actual types in case of a constructed generic type.</summary>
        public IReadOnlyList<BaseTypeReference> GenericArguments { get; internal set; }

        /// <summary>The declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyReference Assembly
            => Namespace.Assembly;

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitType(this);

        /// <summary>Determines whether the current <see cref="TypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => Equals(type, null, null);

        internal override bool Equals(Type type, GenericMethodParameterReference originator, Type originatorMatch)
            => type != null
               && !type.IsPointer
               && !type.IsArray
               && !type.IsByRef
               && !type.IsGenericParameter
               && Name.Equals(type.GetTypeName(), StringComparison.OrdinalIgnoreCase)
               && Namespace.Name.Equals(type.Namespace, StringComparison.OrdinalIgnoreCase)
               && (DeclaringType is null ? type.DeclaringType is null : DeclaringType.Equals(type.GetDeclaringType(), originator, originatorMatch))
               && GenericArguments.Count == type.GetCurrentGenericArguments().Count()
               && (
                    type.IsConstructedGenericType
                        ? GenericArguments
                           .Zip(type.GetCurrentGenericArguments(), (expectedGenericArgument, actualGenericArgument) => (ExpectedGenericArgument: expectedGenericArgument, ActualGenericArgument: actualGenericArgument))
                           .All(pair => pair.ExpectedGenericArgument.Equals(pair.ActualGenericArgument, originator, originatorMatch))
                        : GenericArguments
                            .All(genericArgument => genericArgument is GenericTypeParameterReference genericTypeParameter && ReferenceEquals(this, genericTypeParameter.DeclaringType))
               )
               && Assembly.Equals(type.Assembly);
    }
}
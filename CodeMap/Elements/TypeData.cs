using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.Elements
{
    /// <summary>Represents a reference to a concrete type.</summary>
    public class TypeData : TypeReferenceData
    {
        internal TypeData()
        {
        }

        /// <summary>The type name.</summary>
        public string Name { get; internal set; }

        /// <summary>The type namespace.</summary>
        public string Namespace { get; internal set; }

        /// <summary>The type generic arguments.</summary>
        public IReadOnlyList<TypeReferenceData> GenericArguments { get; internal set; }

        /// <summary>The declaring type.</summary>
        public TypeData DeclaringType { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyReference Assembly { get; internal set; }

        /// <summary>Determines whether the current <see cref="TypeData"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeData"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
        {
            if (type == null || type.IsPointer || type.IsArray || type.IsByRef || type.IsGenericParameter || (type.IsGenericType && !type.IsConstructedGenericType))
                return false;

            var backTickIndex = type.Name.LastIndexOf('`');
            var genericArgumentsOffset = type.DeclaringType?.GetGenericArguments().Length ?? 0;
            return
                string.Equals(Name, (backTickIndex >= 0 ? type.Name.Substring(0, backTickIndex) : type.Name), StringComparison.OrdinalIgnoreCase)
                && string.Equals(Namespace, type.Namespace, StringComparison.OrdinalIgnoreCase)
                && DeclaringType == type.DeclaringType?.MakeGenericType(
                    type
                        .GetGenericArguments()
                        .Take(genericArgumentsOffset)
                        .ToArray()
                )
                && GenericArguments.Count == type.GetGenericArguments().Length - genericArgumentsOffset
                && GenericArguments
                    .Zip(
                        type.GetGenericArguments().Skip(genericArgumentsOffset),
                        (typeReference, genericArgument) => new { TypeReference = typeReference, GenericArgument = genericArgument }
                    )
                    .All(pair => pair.TypeReference == pair.GenericArgument)
                && Assembly == type.Assembly.GetName();
        }
    }
}
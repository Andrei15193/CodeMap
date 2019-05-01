using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.ReferenceData
{
    internal static class Extensions
    {
        public static ReadOnlySpan<char> GetTypeName(this Type type)
        {
            var backTickIndex = type.Name.LastIndexOf('`');
            if (backTickIndex >= 0)
                return type.Name.AsSpan(0, backTickIndex);
            else
                return type.Name;
        }

        public static Type GetDeclaringType(this Type type)
        {
            if (type.DeclaringType == null)
                return null;

            var genericArgumentsOffset = type.DeclaringType.GetGenericArguments().Length;
            if (type.IsGenericType && type.IsConstructedGenericType)
                return type.DeclaringType.MakeGenericType(
                    type
                        .GetGenericArguments()
                        .Take(genericArgumentsOffset)
                        .ToArray()
                );
            else
                return type.DeclaringType;
        }

        public static IEnumerable<Type> GetCurrentGenericArguments(this Type type)
        {
            if (type.DeclaringType == null)
                return type.GetGenericArguments();
            else
                return type.GetGenericArguments().Skip(type.DeclaringType.GetGenericArguments().Length);
        }
    }
}
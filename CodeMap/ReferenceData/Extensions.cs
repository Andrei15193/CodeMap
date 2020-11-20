using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    internal static class Extensions
    {
        public static string GetTypeName(this Type type)
        {
            var backTickIndex = type.Name.LastIndexOf('`');
            if (backTickIndex >= 0)
                return type.Name.Substring(0, backTickIndex);
            else
                return type.Name;
        }

        public static string GetMethodName(this MethodBase methodBase)
        {
            var backTickIndex = methodBase.Name.LastIndexOf("``", StringComparison.OrdinalIgnoreCase);
            if (backTickIndex >= 0)
                return methodBase.Name.Substring(0, backTickIndex);
            else
                return methodBase.Name;
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
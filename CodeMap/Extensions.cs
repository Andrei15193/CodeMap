using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeMap
{
    internal static class Extensions
    {
        private static class Cache<TKey, TValue>
        {
            public static readonly IReadOnlyDictionary<TKey, TValue> Empty = new Dictionary<TKey, TValue>();
        }

        public static IReadOnlyDictionary<TKey, TValue> EmptyDictionary<TKey, TValue>()
            => Cache<TKey, TValue>.Empty;

        public static IReadOnlyList<TItem> ToReadOnlyList<TItem>(this IEnumerable<TItem> items)
            => items is null
            ? null
            : items is IReadOnlyList<TItem> readOnlyList
            ? readOnlyList
            : items.Any()
            ? items.ToArray()
            : Array.Empty<TItem>();

        public static string ToBase16String(this IEnumerable<byte> bytes)
        {
            if (bytes is null)
                return null;

            var result = (bytes is IReadOnlyCollection<byte> byteCollection)
                ? new StringBuilder(byteCollection.Count * 2)
                : new StringBuilder();
            foreach (var @byte in bytes)
                result.Append(@byte.ToString("x2"));
            return result.ToString();
        }

        public static StringBuilder Join<T>(this StringBuilder stringBuilder, char separator, IEnumerable<T> values, Action<T> callback)
        {
            using (var value = values.GetEnumerator())
                if (value.MoveNext())
                {
                    callback(value.Current);
                    while (value.MoveNext())
                    {
                        stringBuilder.Append(separator);
                        callback(value.Current);
                    }
                }
            return stringBuilder;
        }

        public static IEnumerable<Type> GetNestingChain(this Type type)
        {
            var currentType = type;
            var declaringTypes = new LinkedList<Type>();
            do
            {
                declaringTypes.AddFirst(currentType);
                currentType = currentType.DeclaringType;
            } while (currentType is object);
            return declaringTypes;
        }

        public static IEnumerable<Type> GetAllDefinedTypes(this Assembly assembly)
        {
            var toVisit = new Queue<Type>(assembly.DefinedTypes.Where(type => type.DeclaringType is null));
            while (toVisit.Count > 0)
            {
                var type = toVisit.Dequeue();
                foreach (var nestedType in type.GetNestedTypes())
                    toVisit.Enqueue(nestedType);
                yield return type;
            }
        }

        public static IEnumerable<Type> GetAllForwardedTypes(this Assembly assembly)
        {
            var toVisit = new Queue<Type>(assembly.GetForwardedTypes().Where(type => type.DeclaringType is null));
            while (toVisit.Count > 0)
            {
                var type = toVisit.Dequeue();
                foreach (var nestedType in type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public))
                    toVisit.Enqueue(nestedType);
                yield return type;
            }
        }
    }
}
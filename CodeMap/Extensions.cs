using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeMap
{
    internal static class Extensions
    {
        public static string ToBase16String(this IEnumerable<byte> bytes)
        {
            if (bytes == null)
                return null;

            var result = (bytes is IReadOnlyCollection<byte> byteCollection)
                ? new StringBuilder(byteCollection.Count * 2)
                : new StringBuilder();
            foreach (var @byte in bytes)
                result.Append(@byte.ToString("x2"));
            return result.ToString();
        }

        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                return null;

            if (collection is IReadOnlyCollection<T> readOnlyCollection)
                return readOnlyCollection;

            if (collection is ICollection<T> actualCollection)
                return new ReadOnlyCollection<T>(actualCollection);

            return collection.ToList();
        }

        public static IReadOnlyCollection<T> AsReadOnlyCollectionOrEmpty<T>(this IEnumerable<T> collection)
            => collection.AsReadOnlyCollection()
            ?? Enumerable.Empty<T>().AsReadOnlyCollection();

        public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                return null;

            if (collection is IReadOnlyList<T> readOnlyList)
                return readOnlyList;

            if (collection is IList<T> list)
                return new System.Collections.ObjectModel.ReadOnlyCollection<T>(list);

            return collection.ToList();
        }

        public static IReadOnlyList<T> AsReadOnlyListOrEmpty<T>(this IEnumerable<T> collection)
            => collection.AsReadOnlyList()
            ?? Enumerable.Empty<T>().AsReadOnlyList();

        public static ILookup<TKey, TElement> OrEmpty<TKey, TElement>(this ILookup<TKey, TElement> collection)
            => collection ?? new EmptyLookup<TKey, TElement>();

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
            } while (currentType != null);
            return declaringTypes;
        }

        public static IEnumerable<Type> GetAllDefinedTypes(this Assembly assembly)
        {
            var toVisit = new Queue<Type>(assembly.DefinedTypes);
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
            var toVisit = new Queue<Type>(assembly.GetForwardedTypes());
            while (toVisit.Count > 0)
            {
                var type = toVisit.Dequeue();
                foreach (var nestedType in type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public))
                    toVisit.Enqueue(nestedType);
                yield return type;
            }
        }

        public static IEnumerable<Type> GetAllDefinedAndForwardedTypes(this Assembly assembly)
        {
            var toVisit = new Stack<Type>(assembly.GetForwardedTypes().Reverse().Concat(assembly.DefinedTypes.Reverse()));
            while (toVisit.Count > 0)
            {
                var type = toVisit.Pop();
                foreach (var nestedType in type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public))
                    toVisit.Push(nestedType);
                yield return type;
            }
        }

        private sealed class ReadOnlyCollection<T> : IReadOnlyCollection<T>
        {
            private readonly ICollection<T> _collection;

            public ReadOnlyCollection(ICollection<T> collection)
            {
                _collection = collection;
            }

            public int Count
                => _collection.Count;

            public IEnumerator<T> GetEnumerator()
                => _collection.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => ((IEnumerable)_collection).GetEnumerator();
        }

        private sealed class EmptyLookup<TKey, TElement> : ILookup<TKey, TElement>
        {
            public IEnumerable<TElement> this[TKey key]
                => throw new KeyNotFoundException();

            public int Count
                => 0;

            public bool Contains(TKey key)
                => false;

            public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }
    }
}
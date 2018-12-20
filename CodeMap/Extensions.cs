using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap
{
    internal static class Extensions
    {
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
    }
}
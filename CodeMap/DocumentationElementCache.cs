using System;
using System.Collections.Generic;
using System.Reflection;
using CodeMap.Elements;

namespace CodeMap
{
    internal class DocumentationElementCache
    {
        private readonly IDictionary<AssemblyName, AssemblyReference> _assemblyReferencesCache =
            new Dictionary<AssemblyName, AssemblyReference>(new AssemblyNameEqualityComparer());
        private readonly IDictionary<Type, TypeReferenceData> _typeReferencesCache =
            new Dictionary<Type, TypeReferenceData>();

        public AssemblyReference GetFor(AssemblyName assemblyName, Func<AssemblyName, AssemblyReference> factory)
        {
            if (!_assemblyReferencesCache.TryGetValue(assemblyName, out var assemblyReference))
            {
                assemblyReference = factory(assemblyName);
                _assemblyReferencesCache.Add(assemblyName, assemblyReference);
            }
            return assemblyReference;
        }

        public TypeReferenceData GetFor(Type type, Func<Type, TypeReferenceData> factory, Action<Type, TypeReferenceData> initializer)
        {
            if (!_typeReferencesCache.TryGetValue(type, out var typeReference))
            {
                typeReference = factory(type);
                _typeReferencesCache.Add(type, typeReference);
                initializer(type, typeReference);
            }
            return typeReference;
        }

        private sealed class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
        {
            public bool Equals(AssemblyName x, AssemblyName y)
            {
                if (x == null)
                    return y == null;

                return y != null
                    && AssemblyName.ReferenceMatchesDefinition(x, y)
                    && string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                    && x.Version == y.Version
                    && string.Equals(x.CultureName, y.CultureName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.GetPublicKeyToken().ToBase16String(), y.GetPublicKeyToken().ToBase16String(), StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(AssemblyName obj)
                => obj != null
                    ? new
                    {
                        Name = obj.Name?.ToLowerInvariant(),
                        obj.Version,
                        Culture = obj.CultureName?.ToLowerInvariant(),
                        PublicKeyToken = obj.GetPublicKeyToken().ToBase16String()
                    }.GetHashCode()
                    : throw new ArgumentNullException(nameof(obj));
        }
    }
}
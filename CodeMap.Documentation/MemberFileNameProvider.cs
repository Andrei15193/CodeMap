using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation
{
    public class MemberFileNameProvider
    {
        private readonly Dictionary<object, string> _cache = new Dictionary<object, string>(new DeclarationNodeEqualityComparer());

        public string GetFileName(AssemblyDeclaration _)
            => "Index";

        public string GetFileName(NamespaceDeclaration namespaceDeclaration)
            => namespaceDeclaration.Name;

        public string GetFileName(TypeDeclaration typeDeclaration)
            => _cache.TryGetValue(typeDeclaration, out var fileName)
            ? fileName
            : _GetFileName(typeDeclaration, typeDeclaration.GetMemberFullName());

        public string GetFileName(MemberDeclaration memberDeclaration)
            => _cache.TryGetValue(memberDeclaration, out var fileName)
            ? fileName
            : _GetFileName(memberDeclaration, memberDeclaration.GetMemberFullName());

        public string GetFileName(MemberReference memberReference)
            => _cache.TryGetValue(memberReference, out var fileName)
            ? fileName
            : _GetFileName(memberReference, memberReference.GetMemberFullName());

        private string _GetFileName(object key, string baseFileName)
        {
            var count = 1;
            var fileName = baseFileName;
            while (_cache.Values.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            {
                count++;
                fileName = $"{baseFileName}-{count}";
            }
            _cache.Add(key, fileName);
            return fileName;
        }

        private sealed class DeclarationNodeEqualityComparer : IEqualityComparer, IEqualityComparer<object>
        {
            public new bool Equals(object x, object y)
                => x is MemberReference && x is MemberReference
                ? object.Equals(x, y)
                : y is MemberReference
                ? x switch
                {
                    MemberDeclaration memberDeclaration => memberDeclaration.Equals((MemberReference)y),
                    TypeDeclaration typeDeclaration => typeDeclaration.Equals((MemberReference)y),
                    _ => Equals(y, (MemberReference)x)
                }
                : y is TypeDeclaration
                ? x switch
                {
                    MemberDeclaration memberDeclaration => memberDeclaration.Equals((TypeDeclaration)y),
                    TypeDeclaration typeDeclaration => typeDeclaration.Equals((TypeDeclaration)y),
                    _ => Equals(y, (MemberReference)x)
                }
                : x switch
                {
                    MemberDeclaration memberDeclaration => memberDeclaration.Equals((MemberDeclaration)y),
                    TypeDeclaration typeDeclaration => typeDeclaration.Equals((MemberDeclaration)y),
                    _ => Equals(y, (MemberReference)x)
                };

            public int GetHashCode(object obj)
                => 0;
        }
    }
}
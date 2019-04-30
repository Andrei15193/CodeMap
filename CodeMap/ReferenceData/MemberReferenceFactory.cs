﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a <see cref="MemberReference"/> factory.</summary>
    public class MemberReferenceFactory
    {
        private readonly IDictionary<AssemblyName, AssemblyReference> _cachedAssemblyReferences = new Dictionary<AssemblyName, AssemblyReference>(new AssemblyNameEqualityComparer());
        private readonly IDictionary<MemberInfo, MemberReference> _cachedMemberReferences = new Dictionary<MemberInfo, MemberReference>();

        /// <summary>Initializes a new instance of the <see cref="MemberReferenceFactory"/> class.</summary>
        public MemberReferenceFactory()
        {
            _cachedAssemblyReferences = new Dictionary<AssemblyName, AssemblyReference>(new AssemblyNameEqualityComparer());
        }

        /// <summary>Creates an <see cref="MemberReference"/> for the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> for which to create the reference.</param>
        /// <returns>Returns an <see cref="MemberReference"/> for the provided <paramref name="memberInfo"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="memberInfo"/> is <c>null</c>.</exception>
        public MemberReference Create(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                throw new ArgumentNullException(nameof(memberInfo));

            switch (memberInfo)
            {
                case Type type:
                    return new TypeReference
                    {
                        Name = type.Name,
                        Namespace = type.Namespace,
                        GenericArguments = Enumerable.Empty<BaseTypeReference>().AsReadOnlyList(),
                        Assembly = Create(type.Assembly)
                    };

                default:
                    throw new ArgumentException("Unknown member type.", nameof(memberInfo));
            }
        }

        /// <summary>Creates an <see cref="AssemblyReference"/> for the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to create the reference.</param>
        /// <returns>Returns an <see cref="AssemblyReference"/> for the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        public AssemblyReference Create(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return Create(assembly.GetName());
        }

        /// <summary>Creates an <see cref="AssemblyReference"/> for the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> for which to create the reference.</param>
        /// <returns>Returns an <see cref="AssemblyReference"/> for the provided <paramref name="assemblyName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assemblyName"/> is <c>null</c>.</exception>
        public AssemblyReference Create(AssemblyName assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));

            if (!_cachedAssemblyReferences.TryGetValue(assemblyName, out var assemblyReference))
            {
                assemblyReference = _GetAssemblyReference(assemblyName);
                _cachedAssemblyReferences.Add(assemblyName, assemblyReference);
            }
            return assemblyReference;
        }

        private static AssemblyReference _GetAssemblyReference(AssemblyName assemblyName)
            => new AssemblyReference
            {
                Name = assemblyName.Name,
                Version = assemblyName.Version,
                Culture = assemblyName.CultureName,
                PublicKeyToken = assemblyName.GetPublicKeyToken().ToBase16String()
            };

        private sealed class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
        {
            public bool Equals(AssemblyName left, AssemblyName right)
            {
                if (left == null)
                    return right == null;

                return right != null
                    && AssemblyName.ReferenceMatchesDefinition(left, right)
                    && string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase)
                    && left.Version == right.Version
                    && string.Equals(left.CultureName, right.CultureName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(left.GetPublicKeyToken().ToBase16String(), right.GetPublicKeyToken().ToBase16String(), StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(AssemblyName asseblyName)
                => asseblyName != null
                    ? new
                    {
                        Name = asseblyName.Name?.ToLowerInvariant(),
                        asseblyName.Version,
                        Culture = asseblyName.CultureName?.ToLowerInvariant(),
                        PublicKeyToken = asseblyName.GetPublicKeyToken().ToBase16String()
                    }.GetHashCode()
                    : throw new ArgumentNullException(nameof(asseblyName));
        }
    }
}
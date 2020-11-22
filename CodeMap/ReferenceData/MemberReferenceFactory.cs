using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static CodeMap.Extensions;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a <see cref="MemberReference"/> factory.</summary>
    public class MemberReferenceFactory
    {
        private DynamicTypeReference _dynamicTypeReference;
        private readonly IDictionary<AssemblyName, AssemblyReference> _cachedAssemblyReferences;
        private readonly IDictionary<MemberInfo, MemberReference> _cachedMemberReferences;
        private readonly IDictionary<NamespaceCacheKey, NamespaceReference> _cachedNamespaceReferences;
        private readonly IDictionary<Type, ConstructorReference> _cachedDefaultConstructorReferences;

        /// <summary>Initializes a new instance of the <see cref="MemberReferenceFactory"/> class.</summary>
        public MemberReferenceFactory()
        {
            var assemblyNameEqualityComparer = new AssemblyNameEqualityComparer();

            _dynamicTypeReference = null;
            _cachedAssemblyReferences = new Dictionary<AssemblyName, AssemblyReference>(assemblyNameEqualityComparer);
            _cachedMemberReferences = new Dictionary<MemberInfo, MemberReference>();
            _cachedNamespaceReferences = new Dictionary<NamespaceCacheKey, NamespaceReference>(new NamespaceCacheKeyEqualityComparer(assemblyNameEqualityComparer));
            _cachedDefaultConstructorReferences = new Dictionary<Type, ConstructorReference>();
        }

        /// <summary>Creates an <see cref="MemberReference"/> for the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> for which to create the reference.</param>
        /// <returns>Returns an <see cref="MemberReference"/> for the provided <paramref name="memberInfo"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="memberInfo"/> is <c>null</c>.</exception>
        public MemberReference Create(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                throw new ArgumentNullException(nameof(memberInfo));

            if (!_cachedMemberReferences.TryGetValue(memberInfo, out var memberReference))
            {
                Action circularReferenceSetter;
                (memberReference, circularReferenceSetter) = _GetMemberReference(memberInfo);
                _cachedMemberReferences.Add(memberInfo, memberReference);
                circularReferenceSetter();
            }
            return memberReference;
        }

        /// <summary>Creates a <see cref="ConstructorReference"/> for the default constructor of the given <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> for which to create a <see cref="ConstructorReference"/>, must be a <c>struct</c> (value type).</param>
        /// <returns>Returns a <see cref="ConstructorReference"/> for the default constructor of the given <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not a <c>struct</c> (value type).</exception>
        public ConstructorReference CreateDefaultConstructor(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsValueType || type.IsEnum)
                throw new ArgumentException("Default constructor references can only be created for structs (value types).", nameof(type));

            if (!_cachedDefaultConstructorReferences.TryGetValue(type, out var constructorReference))
            {
                constructorReference = new ConstructorReference
                {
                    DeclaringType = (TypeReference)Create(type),
                    ParameterTypes = EmptyReadOnlyList<BaseTypeReference>()
                };
                _cachedDefaultConstructorReferences.Add(type, constructorReference);
            }
            return constructorReference;
        }

        /// <summary>Creates a <see cref="BaseTypeReference"/> for the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> for which to create the reference.</param>
        /// <returns>Returns an <see cref="BaseTypeReference"/> for the provided <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.</exception>
        public BaseTypeReference Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return (BaseTypeReference)Create(memberInfo: type);
        }

        /// <summary>Creates a <see cref="DynamicTypeReference"/> that can be used to represent dynamic typed parameters.</summary>
        /// <returns>Returns a <see cref="DynamicTypeReference"/>.</returns>
        public DynamicTypeReference CreateDynamic()
        {
            if (_dynamicTypeReference is null)
            {
                _dynamicTypeReference = new DynamicTypeReference
                {
                    GenericArguments = EmptyReadOnlyList<GenericTypeParameterReference>()
                };
                _InitializeTypeReference(typeof(object), _dynamicTypeReference);
            }
            return _dynamicTypeReference;
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

        /// <summary>Creates a <see cref="NamespaceReference"/> for the provided namespace <paramref name="name"/> and <paramref name="assembly"/>.</summary>
        /// <param name="name">The name of the namespace.</param>
        /// <param name="assembly">The <see cref="Assembly"/> in which the namespace is declared.</param>
        /// <returns>Returns an <see cref="Assembly"/> for the provided namespace <paramref name="name"/> and <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        public NamespaceReference CreateNamespace(string name, Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return CreateNamespace(name, assembly.GetName());
        }

        /// <summary>Creates a <see cref="NamespaceReference"/> for the provided namespace <paramref name="name"/> and <paramref name="assemblyName"/>.</summary>
        /// <param name="name">The name of the namespace.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> in which the namespace is declared.</param>
        /// <returns>Returns an <see cref="AssemblyName"/> for the provided namespace <paramref name="name"/> and <paramref name="assemblyName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assemblyName"/> is <c>null</c>.</exception>
        public NamespaceReference CreateNamespace(string name, AssemblyName assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));

            var namespaceName = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
            var cacheKey = new NamespaceCacheKey(namespaceName, assemblyName);
            if (!_cachedNamespaceReferences.TryGetValue(cacheKey, out var namespaceReference))
            {
                namespaceReference = new NamespaceReference(namespaceName, Create(assemblyName));
                _cachedNamespaceReferences.Add(cacheKey, namespaceReference);
            }
            return namespaceReference;
        }

        private (MemberReference MemberReference, Action CircularReferenceSetter) _GetMemberReference(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case Type type when type.IsArray:
                    return _GetArrayTypeReference(type);

                case Type type when type.IsPointer:
                    return _GetPointerTypeReference(type);

                case Type type when type.IsByRef:
                    return _GetByRefTypeReference(type);

                case Type type when type.IsGenericTypeParameter:
                    return _GetGenericTypeParameterReference(type);

                case Type type when type.IsGenericMethodParameter:
                    return _GetGenericMethodParameterReference(type);

                case Type type:
                    return _GetTypeReference(type);

                case FieldInfo fieldInfo when fieldInfo.IsLiteral:
                    return _GetConstantReference(fieldInfo);

                case FieldInfo fieldInfo:
                    return _GetFieldReference(fieldInfo);

                case ConstructorInfo constructorInfo:
                    return _GetConstructorReference(constructorInfo);

                case EventInfo eventInfo:
                    return _GetEventReference(eventInfo);

                case PropertyInfo propertyInfo:
                    return _GetPropertyReference(propertyInfo);

                case MethodInfo methodInfo:
                    return _GetMethodReference(methodInfo);

                default:
                    throw new ArgumentException("Unknown member type.", nameof(memberInfo));
            }
        }

        private (MemberReference MemberReference, Action CircularReferenceSetter) _GetTypeReference(Type type)
        {
            var typeReference = type == typeof(void)
                ? new VoidTypeReference()
                : new TypeReference();
            _InitializeTypeReference(type, typeReference);
            return (
                typeReference,
                () =>
                {
                    var declaringType = type.GetDeclaringType();
                    typeReference.DeclaringType = declaringType != null
                       ? (TypeReference)Create(declaringType)
                       : null;
                    typeReference.GenericArguments = type
                        .GetCurrentGenericArguments()
                        .Select(Create)
                        .Cast<BaseTypeReference>()
                        .ToReadOnlyList();
                }
            );
        }

        private void _InitializeTypeReference(Type type, TypeReference typeReference)
        {
            typeReference.Name = type.GetTypeName();
            typeReference.Namespace = CreateNamespace(type.Namespace, type.Assembly);
        }

        private (ArrayTypeReference MemberReference, Action CircularReferenceSetter) _GetArrayTypeReference(Type type)
        {
            var arrayTypeReference = new ArrayTypeReference
            {
                Rank = type.GetArrayRank()
            };
            return (
                arrayTypeReference,
                () => arrayTypeReference.ItemType = Create(type.GetElementType())
            );
        }

        private (PointerTypeReference MemberReference, Action CircularReferenceSetter) _GetPointerTypeReference(Type type)
        {
            var pointerTypeReference = new PointerTypeReference();
            return (
                pointerTypeReference,
                () => pointerTypeReference.ReferentType = Create(type.GetElementType())
            );
        }

        private (ByRefTypeReference MemberReference, Action CircularReferenceSetter) _GetByRefTypeReference(Type type)
        {
            var byRefTypeReference = new ByRefTypeReference();
            return (
                byRefTypeReference,
                () => byRefTypeReference.ReferentType = Create(type.GetElementType())
            );
        }

        private (GenericTypeParameterReference MemberReference, Action CircularReferenceSetter) _GetGenericTypeParameterReference(Type type)
        {
            var genericTypeParameterReference = new GenericTypeParameterReference
            {
                Name = type.Name,
                Position = type.GenericParameterPosition
            };
            return (
                genericTypeParameterReference,
                () => genericTypeParameterReference.DeclaringType = (TypeReference)Create(type.DeclaringType)
            );
        }

        private (GenericMethodParameterReference MemberReference, Action CircularReferenceSetter) _GetGenericMethodParameterReference(Type type)
        {
            var genericMethodParameterReference = new GenericMethodParameterReference
            {
                Name = type.Name,
                Position = type.GenericParameterPosition
            };
            return (
                genericMethodParameterReference,
                () => genericMethodParameterReference.DeclaringMethod = (MethodReference)Create(type.DeclaringMethod)
            );
        }

        private (ConstantReference MemberReference, Action CircularReferenceSetter) _GetConstantReference(FieldInfo fieldInfo)
        {
            var constantReference = new ConstantReference
            {
                Name = fieldInfo.Name,
                Value = fieldInfo.GetValue(null)
            };
            return (
                constantReference,
                () => constantReference.DeclaringType = (TypeReference)Create(fieldInfo.DeclaringType)
            );
        }

        private (FieldReference MemberReference, Action CircularReferenceSetter) _GetFieldReference(FieldInfo fieldInfo)
        {
            var constantReference = new FieldReference
            {
                Name = fieldInfo.Name
            };
            return (
                constantReference,
                () => constantReference.DeclaringType = (TypeReference)Create(fieldInfo.DeclaringType)
            );
        }

        private (ConstructorReference MemberReference, Action CircularReferenceSetter) _GetConstructorReference(ConstructorInfo constructorInfo)
        {
            var constructorReference = new ConstructorReference();
            return (
                constructorReference,
                () =>
                {
                    constructorReference.DeclaringType = (TypeReference)Create(constructorInfo.DeclaringType);
                    constructorReference.ParameterTypes = constructorInfo
                        .GetParameters()
                        .Select(parameter => Create(parameter.ParameterType))
                        .Cast<BaseTypeReference>()
                        .ToReadOnlyList();
                }
            );
        }

        private (EventReference MemberReference, Action CircularReferenceSetter) _GetEventReference(EventInfo eventInfo)
        {
            var eventReference = new EventReference
            {
                Name = eventInfo.Name
            };
            return (
                eventReference,
                () => eventReference.DeclaringType = (TypeReference)Create(eventInfo.DeclaringType)
            );
        }

        private (PropertyReference MemberReference, Action CircularReferenceSetter) _GetPropertyReference(PropertyInfo propertyInfo)
        {
            var propertyReference = new PropertyReference
            {
                Name = propertyInfo.Name
            };
            return (
                propertyReference,
                () =>
                {
                    propertyReference.DeclaringType = (TypeReference)Create(propertyInfo.DeclaringType);
                    propertyReference.ParameterTypes = propertyInfo
                        .GetIndexParameters()
                        .Select(parameter => Create(parameter.ParameterType))
                        .Cast<BaseTypeReference>()
                        .ToReadOnlyList();
                }
            );
        }

        private (MethodReference MemberReference, Action CircularReferenceSetter) _GetMethodReference(MethodInfo methodInfo)
        {
            var methodReference = new MethodReference
            {
                Name = methodInfo.Name
            };
            return (
                methodReference,
                () =>
                {
                    methodReference.GenericArguments = methodInfo
                        .GetGenericArguments()
                        .Select(Create)
                        .Cast<BaseTypeReference>()
                        .ToReadOnlyList();
                    methodReference.DeclaringType = (TypeReference)Create(methodInfo.DeclaringType);
                    methodReference.ParameterTypes = methodInfo
                        .GetParameters()
                        .Select(parameter => Create(parameter.ParameterType))
                        .Cast<BaseTypeReference>()
                        .ToReadOnlyList();
                }
            );
        }

        private static AssemblyReference _GetAssemblyReference(AssemblyName assemblyName)
            => new AssemblyReference
            {
                Name = assemblyName.Name,
                Version = assemblyName.Version,
                Culture = assemblyName.CultureName,
                PublicKeyToken = assemblyName.GetPublicKeyToken().ToBase16String()
            };
        private struct NamespaceCacheKey
        {
            public NamespaceCacheKey(string name, AssemblyName assemblyName)
                => (Name, AssemblyName) = (name, assemblyName);

            public string Name { get; }

            public AssemblyName AssemblyName { get; }
        }

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

        private sealed class NamespaceCacheKeyEqualityComparer : IEqualityComparer<NamespaceCacheKey>
        {
            private readonly IEqualityComparer<AssemblyName> _assemblyNameEqualityComparer;

            public NamespaceCacheKeyEqualityComparer(IEqualityComparer<AssemblyName> assemblyNameEqualityComparer)
                => _assemblyNameEqualityComparer = assemblyNameEqualityComparer;

            public bool Equals(NamespaceCacheKey x, NamespaceCacheKey y)
                => string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) && _assemblyNameEqualityComparer.Equals(x.AssemblyName, y.AssemblyName);

            public int GetHashCode(NamespaceCacheKey obj)
                => HashCode.Combine(obj.Name.GetHashCode(StringComparison.OrdinalIgnoreCase), obj.AssemblyName);
        }
    }
}
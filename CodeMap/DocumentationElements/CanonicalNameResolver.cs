using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeMap.DocumentationElements
{
    /// <summary>Resolves XML documentation canonical names from a given <see cref="MemberInfo"/>.</summary>
    public class CanonicalNameResolver
    {
        private const BindingFlags _publicAndNonPublicBindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.IgnoreReturn;
        private const BindingFlags _bestMatchStaticBindingFlags = BindingFlags.Static | _publicAndNonPublicBindingFlags;
        private const BindingFlags _bestMatchInstanceBindingFlags = BindingFlags.Instance | _publicAndNonPublicBindingFlags;
        private const BindingFlags _bestMatchBindingFlags = _bestMatchStaticBindingFlags | _bestMatchInstanceBindingFlags;
        private const BindingFlags _ignoreCaseBindingFlags = _bestMatchBindingFlags | BindingFlags.IgnoreCase;

        private readonly IReadOnlyCollection<Assembly> _searchAssemblies;

        /// <summary>Initializes a new instance of the <see cref="CanonicalNameResolver"/> class.</summary>
        /// <param name="searchAssemblies">The assemblies to search in for <see cref="MemberInfo"/>s.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="searchAssemblies"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="searchAssemblies"/> contains <c>null</c>.</exception>
        public CanonicalNameResolver(IEnumerable<Assembly> searchAssemblies)
        {
            _searchAssemblies = searchAssemblies.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(searchAssemblies));
            if (_searchAssemblies.Contains(null))
                throw new ArgumentException("Cannot contain 'null' assemblies.", nameof(searchAssemblies));
        }

        /// <summary>Gets the XML documentation canonical name for the given <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> for which to get the canonical name.</param>
        /// <returns>Returns the canonical name for the given <paramref name="memberInfo"/>.</returns>
        public string GetCanonicalNameFrom(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case null:
                    throw new ArgumentNullException(nameof(memberInfo));

                case Type type:
                    return _GetTypeCanonicalNameFor(type);

                case FieldInfo fieldInfo:
                    return _GetFieldCanonicalNameFor(fieldInfo);

                case EventInfo eventInfo:
                    return _GetEventCanonicalNameFor(eventInfo);

                case PropertyInfo propertyInfo:
                    return _GetPropertyCanonicalNameFor(propertyInfo);

                case MethodInfo methodInfo:
                    return _GetMethodCanonicalNameFor(methodInfo);

                case ConstructorInfo constructorInfo:
                    return _GetConstructorCanonicalNameFor(constructorInfo);

                default:
                    return null;
            }
        }

        internal string GetDefaultConstructorCanonicalNameFor(Type type)
            => _AppendTypeName(new StringBuilder("M:"), type).Append(".#ctor").ToString();

        /// <summary>Attempts to find a <see cref="MemberInfo"/> with the provided <paramref name="canonicalName"/>.</summary>
        /// <param name="canonicalName">The canonical name of the member to search for.</param>
        /// <returns>Returns the <see cref="MemberInfo"/> with the provided <paramref name="canonicalName"/> if found; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when the canonical name is invalid.</exception>
        public MemberInfo TryFindMemberInfoFor(string canonicalName)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
                throw new ArgumentException("Cannot be 'null', empty or white space.", nameof(canonicalName));
            if (canonicalName.Length < 3)
                throw new ArgumentException($"The canonical name must be at least three characters long, '{canonicalName}' given.", nameof(canonicalName));
            if (canonicalName[1] != ':')
                throw new ArgumentException($"The canonical name must be in the form '<member_type_identifier_character>:<canonical_name_identifier>' (e.g.: T:SomeNamespace.SomeClass), '{canonicalName}' given.", nameof(canonicalName));

            var memberFullName = canonicalName.Substring(2);
            switch (canonicalName[0])
            {
                case 't':
                case 'T':
                    return _TryFindType(memberFullName, Enumerable.Empty<Type>().ToReadOnlyList());

                case 'f':
                case 'F':
                    return _TryFindFieldInfo(memberFullName);

                case 'e':
                case 'E':
                    return _TryFindEventInfo(memberFullName);

                case 'p':
                case 'P':
                    return _TryFindPropertyInfo(memberFullName);

                case 'm':
                case 'M':
                    return _TryFindMethodBase(memberFullName);

                default:
                    throw new ArgumentException($"Cannot find member type for '{canonicalName}' canonical name (T, F, E, P or M must be the first character in the canonical name).", nameof(canonicalName));
            }
        }

        private static string _GetTypeCanonicalNameFor(Type type)
            => _AppendTypeName(new StringBuilder("T:"), type).ToString();

        private static string _GetFieldCanonicalNameFor(FieldInfo fieldInfo)
            => _AppendTypeName(new StringBuilder("F:"), fieldInfo.DeclaringType)
                .Append('.')
                .Append(fieldInfo.Name)
                .ToString();

        private static string _GetEventCanonicalNameFor(EventInfo eventInfo)
        {
            var eventCanonicalNameBuilder = _AppendTypeName(new StringBuilder("E:"), eventInfo.DeclaringType)
                .Append('.');
            return _AppendMemberName(eventCanonicalNameBuilder, eventInfo)
                .ToString();
        }

        private static string _GetPropertyCanonicalNameFor(PropertyInfo propertyInfo)
        {
            var propertyCanonicalNameBuilder = _AppendTypeName(new StringBuilder("P:"), propertyInfo.DeclaringType)
                .Append('.');
            _AppendMemberName(propertyCanonicalNameBuilder, propertyInfo);

            var parameters = propertyInfo.GetIndexParameters();
            if (parameters.Length > 0)
            {
                propertyCanonicalNameBuilder
                    .Append('(')
                    .Join(
                        ',',
                        parameters,
                        parameter => _AppendTypeName(propertyCanonicalNameBuilder, parameter.ParameterType)
                    )
                    .Append(')');
            }

            return propertyCanonicalNameBuilder.ToString();
        }

        private static string _GetMethodCanonicalNameFor(MethodInfo methodInfo)
        {
            var methodCanonicalNameBuilder = _AppendTypeName(new StringBuilder("M:"), methodInfo.DeclaringType)
                .Append('.');
            _AppendMemberName(methodCanonicalNameBuilder, methodInfo);

            var genericParameters = methodInfo.GetGenericArguments();
            if (genericParameters.Length > 0)
                methodCanonicalNameBuilder
                    .Append("``")
                    .Append(genericParameters.Length);

            var parameters = methodInfo.GetParameters();
            if (parameters.Length > 0)
            {
                methodCanonicalNameBuilder
                    .Append('(')
                    .Join(
                        ',',
                        parameters,
                        parameter => _AppendTypeName(methodCanonicalNameBuilder, parameter.ParameterType)
                    )
                    .Append(')');
            }

            return methodCanonicalNameBuilder.ToString();
        }

        private static string _GetConstructorCanonicalNameFor(ConstructorInfo constructorInfo)
        {
            var methodCanonicalNameBuilder = _AppendTypeName(new StringBuilder("M:"), constructorInfo.DeclaringType)
                .Append(constructorInfo.IsStatic ? ".#cctor" : ".#ctor");

            var parameters = constructorInfo.GetParameters();
            if (parameters.Length > 0)
            {
                methodCanonicalNameBuilder
                    .Append('(')
                    .Join(
                        ',',
                        parameters,
                        parameter => _AppendTypeName(methodCanonicalNameBuilder, parameter.ParameterType)
                    )
                    .Append(')');
            }

            return methodCanonicalNameBuilder.ToString();
        }

        private static StringBuilder _AppendTypeName(StringBuilder stringBuilder, Type type, char identifierSeparator = '.')
        {
            if (type.IsByRef)
                return _AppendTypeName(stringBuilder, type.GetElementType())
                    .Append('@');
            else if (type.IsPointer)
                return _AppendTypeName(stringBuilder, type.GetElementType())
                    .Append('*');
            else if (type.IsArray)
            {
                var arrayRank = type.GetArrayRank();
                if (arrayRank == 1)
                    return _AppendTypeName(stringBuilder, type.GetElementType())
                        .Append("[]");
                else
                    return _AppendTypeName(stringBuilder, type.GetElementType())
                        .Append('[')
                        .Join(
                            ',',
                            Enumerable.Range(0, arrayRank),
                            dimension => stringBuilder.Append("0:")
                        )
                        .Append(']');
            }
            else if (type.IsGenericTypeParameter)
                return stringBuilder
                    .Append('`')
                    .Append(type.GenericParameterPosition);
            if (type.IsGenericMethodParameter)
                return stringBuilder
                    .Append("``")
                    .Append(type.GenericParameterPosition);
            else
            {
                if (!string.IsNullOrWhiteSpace(type.Namespace))
                    stringBuilder
                        .Append(type.Namespace.Replace('.', identifierSeparator))
                        .Append(identifierSeparator);

                var genericArgumentOffset = 0;
                var genericArguments = type.GetGenericArguments();
                return stringBuilder.Join(
                    identifierSeparator,
                    type.GetNestingChain(),
                    currentType =>
                    {
                        var backTickIndex = currentType.Name.IndexOf('`');
                        if (type.IsConstructedGenericType && currentType.IsGenericType)
                        {
                            var genericArgumentsCount = currentType.GetGenericArguments().Length;
                            stringBuilder
                                .Append(currentType
                                    .Name
                                    .AsSpan(
                                        0,
                                        backTickIndex >= 0 ? backTickIndex : currentType.Name.Length
                                    )
                                )
                                .Append('{')
                                .Join(
                                    ',',
                                    Enumerable.Range(genericArgumentOffset, genericArgumentsCount - genericArgumentOffset),
                                    genericArgumentIndex => _AppendTypeName(stringBuilder, genericArguments[genericArgumentIndex])
                                )
                                .Append('}');
                            genericArgumentOffset += genericArgumentsCount;
                        }
                        else
                            stringBuilder.Append(currentType.Name);
                    }
                );
            }
        }

        private static StringBuilder _AppendMemberName(StringBuilder stringBuilder, MemberInfo memberInfo)
        {
            Type baseInterface = null;
            switch (memberInfo)
            {
                case MethodInfo methodInfo:
                    baseInterface = _TryGetInterfaceIfExplicitImplementation(methodInfo);
                    break;

                case PropertyInfo propertyInfo:
                    baseInterface = _TryGetInterfaceIfExplicitImplementation(propertyInfo.GetMethod ?? propertyInfo.SetMethod);
                    break;

                case EventInfo eventInfo:
                    baseInterface = _TryGetInterfaceIfExplicitImplementation(eventInfo.AddMethod ?? eventInfo.RemoveMethod);
                    break;
            }
            if (baseInterface != null)
            {
                var memberNameStartIndex = memberInfo.Name.LastIndexOf('.') + 1;
                _AppendTypeName(stringBuilder, baseInterface, '#')
                    .Append('#')
                    .Append(
                        memberInfo.Name,
                        memberNameStartIndex,
                        memberInfo.Name.Length - memberNameStartIndex
                    );
            }
            else
                stringBuilder.Append(memberInfo.Name);

            return stringBuilder;

            Type _TryGetInterfaceIfExplicitImplementation(MethodInfo methodInfo)
                => methodInfo.IsPrivate
                    ? memberInfo
                        .DeclaringType
                        .GetInterfaces()
                        .Select(memberInfo.DeclaringType.GetInterfaceMap)
                        .Where(
                            interfaceMap => interfaceMap.TargetType == methodInfo.DeclaringType
                                && interfaceMap.TargetMethods.Any(targetMethodInfo => methodInfo == targetMethodInfo)
                        )
                        .Select(interfaceMap => interfaceMap.InterfaceType)
                        .SingleOrDefault()
                    : null;
        }

        private Type _TryFindType(ReadOnlySpan<char> typeFullName, IReadOnlyList<Type> genericArguments)
        {
            if (typeFullName.Length <= 0)
                return null;

            switch (typeFullName[typeFullName.Length - 1])
            {
                case '@':
                    return _TryFindType(typeFullName.Slice(0, typeFullName.Length - 1), genericArguments)
                        ?.MakeByRefType();

                case '*':
                    return _TryFindType(typeFullName.Slice(0, typeFullName.Length - 1), genericArguments)
                        ?.MakePointerType();

                case ']':
                    var rank = 1;
                    var startIndex = typeFullName.Length - 2;
                    while (startIndex >= 0 && typeFullName[startIndex] != '[')
                    {
                        if (typeFullName[startIndex] == ',')
                            rank++;
                        startIndex--;
                    }
                    var elementType = _TryFindType(typeFullName.Slice(0, startIndex), genericArguments);
                    return rank == 1 ? elementType.MakeArrayType() : elementType?.MakeArrayType(rank);

                default:
                    if (typeFullName.StartsWith("``", StringComparison.OrdinalIgnoreCase)
                        && int.TryParse(
                            typeFullName.Slice(2),
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out var genericMethodParameterPosition))
                        return Type.MakeGenericMethodParameter(genericMethodParameterPosition);

                    if (typeFullName.StartsWith("`", StringComparison.OrdinalIgnoreCase)
                        && int.TryParse(
                            typeFullName.Slice(1),
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out var genericTypeParameterPosition))
                        return genericArguments[genericTypeParameterPosition];

                    Type result = null;
                    var foundBestMatch = false;

                    var typeConstructionInfo = _GetTypeConstructionInfo(typeFullName);
                    using (var type = _searchAssemblies
                            .SelectMany(Extensions.GetAllDefinedTypes)
                            .Concat(_searchAssemblies.SelectMany(Extensions.GetAllForwardedTypes))
                            .GetEnumerator())
                        while (type.MoveNext() && !foundBestMatch)
                        {
                            var currentTypeFullName = _AppendTypeName(new StringBuilder(), type.Current).ToString();
                            if (string.Equals(currentTypeFullName, typeConstructionInfo.TypeDefinitionFullName, StringComparison.Ordinal))
                            {
                                result = type.Current;
                                foundBestMatch = true;
                            }
                            else if (result == null && string.Equals(currentTypeFullName, typeConstructionInfo.TypeDefinitionFullName, StringComparison.OrdinalIgnoreCase))
                                result = type.Current;
                        }

                    if (typeConstructionInfo.GenericArgumentsFullNames.Any())
                        result = result
                            ?.MakeGenericType(
                                typeConstructionInfo
                                    .GenericArgumentsFullNames
                                    .Select(genericArgumentFullName => _TryFindType(genericArgumentFullName, genericArguments))
                                    .ToArray()
                            );

                    return result;
            }
        }

        private TypeConstructionInfo _GetTypeConstructionInfo(ReadOnlySpan<char> typeFullName)
        {
            var genericArgumentDepth = 0;
            var genericArgumentStartIndex = 0;
            var currentTypeGenericArgumentsCount = 0;
            var genericArguments = new List<string>();
            var genericTypeDefinitionFullNameBuilder = new StringBuilder();
            for (var index = 0; index < typeFullName.Length; index++)
                switch (typeFullName[index])
                {
                    case '{':
                        if (genericArgumentDepth == 0)
                        {
                            currentTypeGenericArgumentsCount = 1;
                            genericArgumentStartIndex = index + 1;
                        }
                        genericArgumentDepth++;
                        break;

                    case ',' when genericArgumentDepth == 1:
                        genericArguments.Add(
                            typeFullName
                                .Slice(
                                    genericArgumentStartIndex,
                                    index - genericArgumentStartIndex
                                )
                                .ToString()
                        );
                        currentTypeGenericArgumentsCount++;
                        genericArgumentStartIndex = index + 1;
                        break;

                    case '}':
                        genericArgumentDepth--;
                        if (genericArgumentDepth == 0)
                        {
                            genericArguments.Add(
                                typeFullName
                                    .Slice(
                                        genericArgumentStartIndex,
                                        index - genericArgumentStartIndex
                                    )
                                    .ToString()
                            );
                            genericTypeDefinitionFullNameBuilder
                                .Append('`')
                                .Append(currentTypeGenericArgumentsCount);
                        }
                        break;

                    default:
                        if (genericArgumentDepth == 0)
                            genericTypeDefinitionFullNameBuilder.Append(typeFullName[index]);
                        break;
                }

            return new TypeConstructionInfo(
                genericTypeDefinitionFullNameBuilder.ToString(),
                genericArguments.AsEnumerable()
            );
        }

        private FieldInfo _TryFindFieldInfo(string memberFullName)
        {
            var match = Regex.Match(
                memberFullName,
                @"^(?<declaringTypeFullName>[^\.]+(\.[^\.]+)*)\.(?<fieldName>[^\.]+)$",
                RegexOptions.ExplicitCapture
            );
            if (!match.Success)
                return null;

            var declaringTypeFullName = match.Groups["declaringTypeFullName"].Value;
            var declaringType = _TryFindType(declaringTypeFullName, Extensions.EmptyReadOnlyList<Type>());
            if (declaringType == null)
                return null;

            var fieldName = match.Groups["fieldName"].Value;
            return declaringType.GetField(fieldName, _bestMatchBindingFlags) ?? declaringType.GetField(fieldName, _ignoreCaseBindingFlags);
        }

        private EventInfo _TryFindEventInfo(string memberFullName)
        {
            var match = Regex.Match(
                memberFullName,
                @"^(?<declaringTypeFullName>[^\.]+(\.[^\.]+)*)\.(?<eventName>[^\.]+)$",
                RegexOptions.ExplicitCapture
            );
            if (!match.Success)
                return null;

            var declaringTypeFullName = match.Groups["declaringTypeFullName"].Value;
            var declaringType = _TryFindType(declaringTypeFullName, Extensions.EmptyReadOnlyList<Type>());
            if (declaringType == null)
                return null;

            var eventName = match.Groups["eventName"].Value.Replace('#', '.');
            return declaringType.GetEvent(eventName, _bestMatchBindingFlags) ?? declaringType.GetEvent(eventName, _ignoreCaseBindingFlags);
        }

        private PropertyInfo _TryFindPropertyInfo(string memberFullName)
        {
            var match = Regex.Match(
                memberFullName,
                @"^ (?<declaringTypeFullName>
                           [^\.()]+
                        (\.[^\.()]+)*
                    )\.
                    (?<propertyName>[^\.()]+)
                    (
                        \(
                              (?<parameterType>`\d+|``\d+|((?(open)(?!))[^,(){}\[\]]|(?(open)[^(){}\[\]])|(?'open'\{)|(?'close-open'\})|\[(0:(,0:)*)?\])+(?(open)(?!)))
                            (,(?<parameterType>`\d+|``\d+|((?(open)(?!))[^,(){}\[\]]|(?(open)[^(){}\[\]])|(?'open'\{)|(?'close-open'\})|\[(0:(,0:)*)?\])+(?(open)(?!))))*
                        \)
                    )?$",
                RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace
            );
            if (!match.Success)
                return null;

            var declaringTypeFullName = match.Groups["declaringTypeFullName"].Value;
            var declaringType = _TryFindType(declaringTypeFullName, Extensions.EmptyReadOnlyList<Type>());
            if (declaringType == null)
                return null;

            var propertyParameterTypes = match
                .Groups["parameterType"]
                .Captures
                .Select(parameterTypeFullName => _TryFindType(parameterTypeFullName.Value, declaringType.GetGenericArguments()))
                .ToArray();
            if (propertyParameterTypes.Any(propertyParameterType => propertyParameterType == null))
                return null;

            var propertyName = match.Groups["propertyName"].Value.Replace('#', '.');
            return declaringType.GetProperty(propertyName, _bestMatchBindingFlags, Type.DefaultBinder, null, propertyParameterTypes, null)
                ?? declaringType.GetProperty(propertyName, _ignoreCaseBindingFlags, Type.DefaultBinder, null, propertyParameterTypes, null);
        }

        private MethodBase _TryFindMethodBase(string memberFullName)
        {
            var match = Regex.Match(
                memberFullName,
                @"^ (?<declaringTypeFullName>
                           [^\.()]+
                        (\.[^\.()]+)*
                    )\.
                    (?<methodName>\#cctor|\#ctor|[^\.()`]+)(``(?<genericParameterCount>\d+))?
                    (
                        \(
                              (?<parameterType>`\d+|``\d+|((?(open)(?!))[^,(){}\[\]]|(?(open)[^(){}\[\]])|(?'open'\{)|(?'close-open'\})|\[(0:(,0:)*)?\])+(?(open)(?!)))
                            (,(?<parameterType>`\d+|``\d+|((?(open)(?!))[^,(){}\[\]]|(?(open)[^(){}\[\]])|(?'open'\{)|(?'close-open'\})|\[(0:(,0:)*)?\])+(?(open)(?!))))*
                        \)
                    )?$",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
            if (!match.Success)
                return null;

            var declaringTypeFullName = match.Groups["declaringTypeFullName"].Value;
            var declaringType = _TryFindType(declaringTypeFullName, Extensions.EmptyReadOnlyList<Type>());
            if (declaringType == null)
                return null;

            var methodParameterTypes = match
                .Groups["parameterType"]
                .Captures
                .Select(parameterTypeFullName => _TryFindType(parameterTypeFullName.Value, declaringType.GetGenericArguments()))
                .ToArray();
            if (methodParameterTypes.Any(propertyParameterType => propertyParameterType == null))
                return null;

            var methodName = match.Groups["methodName"].Value;
            if ("#cctor".Equals(methodName, StringComparison.OrdinalIgnoreCase))
                return declaringType.GetConstructor(_bestMatchStaticBindingFlags, Type.DefaultBinder, methodParameterTypes, null);
            else if ("#ctor".Equals(methodName, StringComparison.OrdinalIgnoreCase))
                return declaringType.GetConstructor(_bestMatchInstanceBindingFlags, Type.DefaultBinder, methodParameterTypes, null);
            else
            {
                int.TryParse(
                    match.Groups["genericParameterCount"].Value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var genericParameterCount
                );
                var searchMethodName = methodName.Replace('#', '.');
                return declaringType.GetMethod(searchMethodName, genericParameterCount, _bestMatchBindingFlags, Type.DefaultBinder, CallingConventions.Any, methodParameterTypes, null)
                    ?? declaringType.GetMethod(searchMethodName, genericParameterCount, _ignoreCaseBindingFlags, Type.DefaultBinder, CallingConventions.Any, methodParameterTypes, null);
            }
        }

        private struct TypeConstructionInfo
        {
            public TypeConstructionInfo(string typeDefinitionFullName, IEnumerable<string> genericArgumentsFullNames)
            {
                TypeDefinitionFullName = typeDefinitionFullName;
                GenericArgumentsFullNames = genericArgumentsFullNames.ToReadOnlyList();
            }

            public string TypeDefinitionFullName { get; }

            public IReadOnlyCollection<string> GenericArgumentsFullNames { get; }
        }
    }
}
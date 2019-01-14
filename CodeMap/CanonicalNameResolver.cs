using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeMap
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
            _searchAssemblies = searchAssemblies.AsReadOnlyCollection()
                ?? throw new ArgumentNullException(nameof(searchAssemblies));
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
                    return _TryFindType(memberFullName);

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
            => _AppendTypeName(new StringBuilder("E:"), eventInfo.DeclaringType)
                .Append('.')
                .Append(eventInfo.Name)
                .ToString();

        private static string _GetPropertyCanonicalNameFor(PropertyInfo propertyInfo)
        {
            var propertyCanonicalNameBuilder = _AppendTypeName(new StringBuilder("P:"), propertyInfo.DeclaringType)
                .Append('.')
                .Append(propertyInfo.Name);

            var parameters = propertyInfo.GetIndexParameters();
            if (parameters.Length > 0)
            {
                propertyCanonicalNameBuilder.Append('(');
                using (var parameter = parameters.AsEnumerable().GetEnumerator())
                    if (parameter.MoveNext())
                    {
                        _AppendTypeName(propertyCanonicalNameBuilder, parameter.Current.ParameterType);
                        while (parameter.MoveNext())
                        {
                            propertyCanonicalNameBuilder.Append(',');
                            _AppendTypeName(propertyCanonicalNameBuilder, parameter.Current.ParameterType);
                        }
                    }
                propertyCanonicalNameBuilder.Append(')');
            }

            return propertyCanonicalNameBuilder.ToString();
        }

        private static string _GetMethodCanonicalNameFor(MethodInfo methodInfo)
        {
            var methodCanonicalNameBuilder = _AppendTypeName(new StringBuilder("M:"), methodInfo.DeclaringType)
                .Append('.')
                .Append(methodInfo.Name);

            var genericParameters = methodInfo.GetGenericArguments();
            if (genericParameters.Length > 0)
                methodCanonicalNameBuilder
                    .Append("``")
                    .Append(genericParameters.Length);

            var parameters = methodInfo.GetParameters();
            if (parameters.Length > 0)
            {
                methodCanonicalNameBuilder.Append('(');
                using (var parameter = parameters.AsEnumerable().GetEnumerator())
                    if (parameter.MoveNext())
                    {
                        _AppendTypeName(methodCanonicalNameBuilder, parameter.Current.ParameterType);
                        while (parameter.MoveNext())
                        {
                            methodCanonicalNameBuilder.Append(',');
                            _AppendTypeName(methodCanonicalNameBuilder, parameter.Current.ParameterType);
                        }
                    }
                methodCanonicalNameBuilder.Append(')');
            }

            return methodCanonicalNameBuilder.ToString();
        }

        private static string _GetConstructorCanonicalNameFor(ConstructorInfo constructorInfo)
        {
            var methodCanonicalNameBuilder = _AppendTypeName(new StringBuilder("M:"), constructorInfo.DeclaringType);

            if (constructorInfo.IsStatic)
                methodCanonicalNameBuilder.Append(".#cctor");
            else
                methodCanonicalNameBuilder.Append(".#ctor");

            var parameters = constructorInfo.GetParameters();
            if (parameters.Length > 0)
            {
                methodCanonicalNameBuilder.Append('(');
                using (var parameter = parameters.AsEnumerable().GetEnumerator())
                    if (parameter.MoveNext())
                    {
                        _AppendTypeName(methodCanonicalNameBuilder, parameter.Current.ParameterType);
                        while (parameter.MoveNext())
                        {
                            methodCanonicalNameBuilder.Append(',');
                            _AppendTypeName(methodCanonicalNameBuilder, parameter.Current.ParameterType);
                        }
                    }
                methodCanonicalNameBuilder.Append(')');
            }

            return methodCanonicalNameBuilder.ToString();
        }

        private static StringBuilder _AppendTypeName(StringBuilder stringBuilder, Type type)
        {
            if (type.IsGenericTypeParameter)
                return stringBuilder.Append('`').Append(type.GenericParameterPosition);
            if (type.IsGenericMethodParameter)
                return stringBuilder.Append("``").Append(type.GenericParameterPosition);

            if (!string.IsNullOrWhiteSpace(type.Namespace))
                stringBuilder.Append(type.Namespace).Append('.');

            if (type.DeclaringType != null)
            {
                var declaringTypes = new Stack<Type>();
                var currentType = type.DeclaringType;
                do
                {
                    declaringTypes.Push(currentType);
                    currentType = currentType.DeclaringType;
                } while (currentType != null);
                do
                    stringBuilder.Append(declaringTypes.Pop().Name).Append('.');
                while (declaringTypes.Count > 0);
            }

            return stringBuilder.Append(type.Name);
        }

        private Type _TryFindType(string typeFullName)
        {
            Type result = null;
            var foundBestMatch = false;

            using (var type = _searchAssemblies.SelectMany(Extensions.GetAllDefinedTypes).Concat(_searchAssemblies.SelectMany(Extensions.GetAllForwardedTypes)).GetEnumerator())
                while (type.MoveNext() && !foundBestMatch)
                {
                    var currentTypeFullName = _AppendTypeName(new StringBuilder(), type.Current).ToString();
                    if (string.Equals(currentTypeFullName, typeFullName, StringComparison.Ordinal))
                    {
                        result = type.Current;
                        foundBestMatch = true;
                    }
                    else if (result == null && string.Equals(currentTypeFullName, typeFullName, StringComparison.OrdinalIgnoreCase))
                        result = type.Current;
                }
            return result;
        }

        private Type _TryFindParameterType(string typeFullName, Type declaringType)
        {
            if (typeFullName.StartsWith("``", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(
                    typeFullName.AsSpan(2),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var genericMethodParameterPosition))
                return Type.MakeGenericMethodParameter(genericMethodParameterPosition);

            if (typeFullName.StartsWith("`", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(
                    typeFullName.AsSpan(1),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var genericTypeParameterPosition))
                return declaringType.GetGenericArguments()[genericTypeParameterPosition];

            return _TryFindType(typeFullName);
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
            var declaringType = _TryFindType(declaringTypeFullName);
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
            var declaringType = _TryFindType(declaringTypeFullName);
            if (declaringType == null)
                return null;

            var eventName = match.Groups["eventName"].Value;
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
                              (?<parameterType>`\d+|[^,()`]+)
                            (,(?<parameterType>`\d+|[^,()`]+))*
                        \)
                    )?$",
                RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace
            );
            if (!match.Success)
                return null;

            var declaringTypeFullName = match.Groups["declaringTypeFullName"].Value;
            var declaringType = _TryFindType(declaringTypeFullName);
            if (declaringType == null)
                return null;

            var propertyParameterTypes = match
                .Groups["parameterType"]
                .Captures
                .Select(parameterTypeFullName => _TryFindParameterType(parameterTypeFullName.Value, declaringType))
                .ToArray();
            if (propertyParameterTypes.Any(propertyParameterType => propertyParameterType == null))
                return null;

            var propertyName = match.Groups["propertyName"].Value;
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
                              (?<parameterType>`\d+|``\d+|[^,()`]+)
                            (,(?<parameterType>`\d+|``\d+|[^,()`]+))*
                        \)
                    )?$",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
            if (!match.Success)
                return null;

            var declaringTypeFullName = match.Groups["declaringTypeFullName"].Value;
            var declaringType = _TryFindType(declaringTypeFullName);
            if (declaringType == null)
                return null;

            var methodParameterTypes = match
                .Groups["parameterType"]
                .Captures
                .Select(parameterTypeFullName => _TryFindParameterType(parameterTypeFullName.Value, declaringType))
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
                return declaringType.GetMethod(methodName, genericParameterCount, _bestMatchBindingFlags, Type.DefaultBinder, CallingConventions.Any, methodParameterTypes, null)
                    ?? declaringType.GetMethod(methodName, genericParameterCount, _ignoreCaseBindingFlags, Type.DefaultBinder, CallingConventions.Any, methodParameterTypes, null);
            }
        }
    }
}
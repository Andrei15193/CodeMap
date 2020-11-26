using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars
{
    /// <summary>Exposes the interface for generating URLs for <see cref="MemberReference"/>s generated with <a href="https://andrei15193.github.io/CodeMap">CodeMap</a>.</summary>
    public class CodeMapMemberReferenceResolver : IMemberReferenceResolver
    {
        private static readonly HashAlgorithm _hashAlgorithm = MD5.Create();
        private readonly string _prefix;

        /// <summary>Initializes a new instance of the <see cref="CodeMapMemberReferenceResolver"/> class.</summary>
        public CodeMapMemberReferenceResolver()
            : this(string.Empty)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CodeMapMemberReferenceResolver"/> class.</summary>
        /// <param name="prefix">A value to prefix files with, useful when creating relative paths.</param>
        public CodeMapMemberReferenceResolver(string prefix)
            => _prefix = prefix ?? string.Empty;

        /// <summary>Gets the URL (relative or absolute) for the given <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to get the URL.</param>
        /// <returns>Returns the URL for the given <paramref name="memberReference"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="memberReference"/> is <c>null</c>.</exception>
        public string GetUrl(MemberReference memberReference)
        {
            var memberReferenceFullNameVisitor = new MemberReferenceFullNameVisitor();
            memberReference.Accept(memberReferenceFullNameVisitor);
            return $"{_prefix}{_GetFileName(memberReferenceFullNameVisitor.Result)}.html";
        }

        private string _GetFileName(string fullName)
        {
            var hashPartStartIndex = fullName.IndexOf('(');
            if (hashPartStartIndex == -1)
                return fullName;
            else
            {
                var basePart = fullName.Substring(0, hashPartStartIndex + 1);
                var hashPart = _hashAlgorithm
                    .ComputeHash(Encoding.Default.GetBytes(fullName, hashPartStartIndex, fullName.Length - hashPartStartIndex))
                    .Aggregate(new StringBuilder(), (builder, @char) => builder.AppendFormat("{0:X2}", @char))
                    .ToString();
                return $"{basePart}-{hashPart}";
            }
        }

        private class MemberReferenceFullNameVisitor : MemberReferenceVisitor
        {
            private readonly StringBuilder _fullNameBuilder;

            public MemberReferenceFullNameVisitor()
                => _fullNameBuilder = new StringBuilder();

            public string Result
                => _fullNameBuilder.ToString();

            protected override void VisitAssembly(AssemblyReference assembly)
                => _fullNameBuilder.Append("index");

            protected override void VisitNamespace(NamespaceReference @namespace)
                => _fullNameBuilder.Append(string.IsNullOrWhiteSpace(@namespace.Name) ? "global-namespace" : @namespace.Name);

            protected override void VisitType(TypeReference type)
            {
                if (type.DeclaringType is object)
                {
                    type.DeclaringType.Accept(this);
                    _fullNameBuilder.Append('.');
                }
                else
                    type.Namespace.Accept(this);

                _fullNameBuilder.Append(_GetTypeName(type));
                if (type.GenericArguments.Any())
                    _fullNameBuilder.Append('`').Append(type.GenericArguments.Count);
            }

            protected override void VisitArray(ArrayTypeReference array)
                => array.ItemType.Accept(this);

            protected override void VisitByRef(ByRefTypeReference byRef)
                => byRef.ReferentType.Accept(this);

            protected override void VisitPointer(PointerTypeReference pointer)
                => pointer.ReferentType.Accept(this);

            protected override void VisitConstant(ConstantReference constant)
            {
                constant.DeclaringType.Accept(this);
                _fullNameBuilder.Append('.').Append(constant.Name);
            }

            protected override void VisitField(FieldReference field)
            {
                field.DeclaringType.Accept(this);
                _fullNameBuilder.Append('.').Append(field.Name);
            }

            protected override void VisitConstructor(ConstructorReference constructor)
            {
                constructor.DeclaringType.Accept(this);
                _fullNameBuilder.Append('.').Append(_GetTypeName(constructor.DeclaringType)).Append('(');
                if (constructor.ParameterTypes.Any())
                {
                    var isFirst = true;
                    foreach (var parameterType in constructor.ParameterTypes)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            _fullNameBuilder.Append(',');
                        parameterType.Accept(this);
                    }
                }
                _fullNameBuilder.Append(')');
            }

            protected override void VisitEvent(EventReference @event)
            {
                @event.DeclaringType.Accept(this);
                _fullNameBuilder.Append('.').Append(@event.Name);
            }

            protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
                => _fullNameBuilder.Append(genericTypeParameter.Name);

            protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
                => _fullNameBuilder.Append(genericMethodParameter.Name);

            protected override void VisitProperty(PropertyReference property)
            {
                property.DeclaringType.Accept(this);
                _fullNameBuilder.Append('.').Append(property.Name);
                if (property.ParameterTypes.Any())
                {
                    _fullNameBuilder.Append('[');
                    var isFirst = true;
                    foreach (var parameterType in property.ParameterTypes)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            _fullNameBuilder.Append(',');
                        parameterType.Accept(this);
                    }
                    _fullNameBuilder.Append(']');
                }
            }

            protected override void VisitMethod(MethodReference method)
            {
                method.DeclaringType.Accept(this);
                _fullNameBuilder.Append('.').Append(method.Name);
                if (method.GenericArguments.Any())
                    _fullNameBuilder.Append('`').Append(method.GenericArguments.Count);
                _fullNameBuilder.Append('(');
                if (method.ParameterTypes.Any())
                {
                    var isFirst = true;
                    foreach (var parameterType in method.ParameterTypes)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            _fullNameBuilder.Append(',');
                        parameterType.Accept(this);
                    }
                }
                _fullNameBuilder.Append(')');
            }

            private static string _GetTypeName(TypeReference typeReference)
            {
                if (typeReference == typeof(void))
                    return "void";
                else if (typeReference == typeof(object))
                    return "object";
                else if (typeReference == typeof(bool))
                    return "bool";
                else if (typeReference == typeof(byte))
                    return "byte";
                else if (typeReference == typeof(sbyte))
                    return "sbyte";
                else if (typeReference == typeof(short))
                    return "short";
                else if (typeReference == typeof(ushort))
                    return "ushort";
                else if (typeReference == typeof(int))
                    return "int";
                else if (typeReference == typeof(uint))
                    return "uint";
                else if (typeReference == typeof(long))
                    return "long";
                else if (typeReference == typeof(float))
                    return "float";
                else if (typeReference == typeof(double))
                    return "double";
                else if (typeReference == typeof(decimal))
                    return "decimal";
                else if (typeReference == typeof(char))
                    return "char";
                else if (typeReference == typeof(string))
                    return "string";
                else if (typeReference is DynamicTypeReference)
                    return "dynamic";
                else
                    return typeReference.Name;
            }
        }
    }
}
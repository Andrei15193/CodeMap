using System.Linq;
using System.Text;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars.Visitors
{
    internal class MemberReferenceFullNameVisitor : MemberReferenceVisitor
    {
        private readonly StringBuilder _fullNameBuilder = new StringBuilder();
        private readonly bool _excludeParameters;

        public MemberReferenceFullNameVisitor(bool excludeParameters)
            => _excludeParameters = excludeParameters;

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
            _fullNameBuilder.Append('.').Append(_GetTypeName(constructor.DeclaringType));
            if (!_excludeParameters && constructor.ParameterTypes.Any())
            {
                _fullNameBuilder.Append('(');
                var isFirst = true;
                foreach (var parameterType in constructor.ParameterTypes)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        _fullNameBuilder.Append(',');
                    parameterType.Accept(this);
                }
                _fullNameBuilder.Append(')');
            }
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
            if (!_excludeParameters && property.ParameterTypes.Any())
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
            if (!_excludeParameters && method.ParameterTypes.Any())
            {
                _fullNameBuilder.Append('(');
                var isFirst = true;
                foreach (var parameterType in method.ParameterTypes)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        _fullNameBuilder.Append(',');
                    parameterType.Accept(this);
                }
                _fullNameBuilder.Append(')');
            }
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
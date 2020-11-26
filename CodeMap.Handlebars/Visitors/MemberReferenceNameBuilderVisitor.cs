using System.Linq;
using System.Text;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars.Visitors
{
    internal class MemberReferenceNameBuilderVisitor : MemberReferenceVisitor
    {
        private readonly StringBuilder _nameBuilder;

        public MemberReferenceNameBuilderVisitor(StringBuilder nameBuilder)
            => _nameBuilder = nameBuilder;

        protected override void VisitArray(ArrayTypeReference array)
        {
            array.ItemType.Accept(this);
            _nameBuilder.Append('[');
            for (var rank = 1; rank < array.Rank; rank++)
                _nameBuilder.Append(',');
            _nameBuilder.Append(']');
        }

        protected override void VisitAssembly(AssemblyReference assembly)
            => _nameBuilder.Append(assembly.Name);

        protected override void VisitNamespace(NamespaceReference @namespace)
            => _nameBuilder.Append(string.IsNullOrWhiteSpace(@namespace.Name) ? "global-namespace" : @namespace.Name);

        protected override void VisitType(TypeReference type)
        {
            _nameBuilder.Append(_GetTypeName(type));
            if (type.GenericArguments.Any())
            {
                _nameBuilder.Append('<');
                var isFirst = true;
                foreach (var genericArgument in type.GenericArguments)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        _nameBuilder.Append(", ");
                    genericArgument.Accept(this);
                }
                _nameBuilder.Append('>');
            }
        }

        protected override void VisitByRef(ByRefTypeReference byRef)
            => byRef.ReferentType.Accept(this);

        protected override void VisitPointer(PointerTypeReference pointer)
        {
            pointer.ReferentType.Accept(this);
            _nameBuilder.Append('*');
        }

        protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            => _nameBuilder.Append(genericTypeParameter.Name);

        protected override void VisitConstant(ConstantReference constant)
            => _nameBuilder.Append(constant.Name);

        protected override void VisitField(FieldReference field)
            => _nameBuilder.Append(field.Name);

        protected override void VisitConstructor(ConstructorReference constructor)
        {
            _nameBuilder.Append(_GetTypeName(constructor.DeclaringType));
            _nameBuilder.Append('(');
            var isFirst = true;
            foreach (var parameterType in constructor.ParameterTypes)
            {
                if (isFirst)
                    isFirst = false;
                else
                    _nameBuilder.Append(", ");
                parameterType.Accept(this);
            }
            _nameBuilder.Append(')');
        }

        protected override void VisitEvent(EventReference @event)
            => _nameBuilder.Append(@event.Name);

        protected override void VisitProperty(PropertyReference property)
        {
            _nameBuilder.Append(property.Name);
            if (property.ParameterTypes.Any())
            {
                _nameBuilder.Append('[');
                var isFirst = true;
                foreach (var parameterType in property.ParameterTypes)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        _nameBuilder.Append(", ");
                    parameterType.Accept(this);
                }
                _nameBuilder.Append(']');
            }
        }

        protected override void VisitMethod(MethodReference method)
        {
            _nameBuilder.Append(method.Name);
            if (method.GenericArguments.Any())
            {
                _nameBuilder.Append('<');
                var isFirstGenericArgument = true;
                foreach (var genericArgument in method.GenericArguments)
                {
                    if (isFirstGenericArgument)
                        isFirstGenericArgument = false;
                    else
                        _nameBuilder.Append(", ");
                    genericArgument.Accept(this);
                }
                _nameBuilder.Append('>');
            }

            _nameBuilder.Append('(');
            var isFirstParameterType = true;
            foreach (var parameter in method.ParameterTypes)
            {
                if (isFirstParameterType)
                    isFirstParameterType = false;
                else
                    _nameBuilder.Append(", ");
                parameter.Accept(this);
            }
            _nameBuilder.Append(')');
        }

        protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
            => _nameBuilder.Append(genericMethodParameter.Name);

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
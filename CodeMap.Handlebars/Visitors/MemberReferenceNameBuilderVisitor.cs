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

        protected override void VisitType(TypeReference type)
        {
            _nameBuilder.Append(type.Name);
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
        {
            constant.DeclaringType.Accept(this);
            _nameBuilder.Append('.').Append(constant.Name);
        }

        protected override void VisitField(FieldReference field)
        {
            field.DeclaringType.Accept(this);
            _nameBuilder.Append('.').Append(field.Name);
        }

        protected override void VisitConstructor(ConstructorReference constructor)
        {
            constructor.DeclaringType.Accept(this);
            _nameBuilder.Append('.').Append(constructor.DeclaringType.Name);
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
        {
            @event.DeclaringType.Accept(this);
            _nameBuilder.Append('.').Append(@event.Name);
        }

        protected override void VisitProperty(PropertyReference property)
        {
            property.DeclaringType.Accept(this);
            _nameBuilder.Append('.').Append(property.Name);
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
            method.DeclaringType.Accept(this);
            _nameBuilder.Append('.').Append(method.Name);
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
    }
}
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

        protected override void VisitType(TypeReference type)
        {
            if (type.DeclaringType != null)
            {
                type.DeclaringType.Accept(this);
                _fullNameBuilder.Append('.');
            }
            else if (!string.IsNullOrWhiteSpace(type.Namespace))
                _fullNameBuilder.Append(type.Namespace).Append('.');

            _fullNameBuilder.Append(type.Name);
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
            _fullNameBuilder.Append('.').Append(constructor.DeclaringType.Name);
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
    }
}
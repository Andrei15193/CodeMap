using System.Linq;
using System.Text;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Visitors
{
    internal class MemberReferenceMicrosoftLinkVisitor : MemberReferenceVisitor
    {
        private readonly StringBuilder _linkBuilder = new StringBuilder("https://docs.microsoft.com/dotnet/api/");

        public string Result
            => _linkBuilder.Append("?view=netstandard-2.1").ToString();

        protected override void VisitArray(ArrayTypeReference array)
            => array.ItemType.Accept(this);

        protected override void VisitAssembly(AssemblyReference assembly)
        {
        }

        protected override void VisitType(TypeReference type)
        {
            if (type.DeclaringType != null)
            {
                type.DeclaringType.Accept(this);
                _linkBuilder.Append('.');
            }
            else
                _linkBuilder.Append(type.Namespace).Append('.');

            _linkBuilder.Append(type.Name);
            if (type.GenericArguments.Any())
            {
                _linkBuilder.Append('-');
                _linkBuilder.Append(type.GenericArguments.Count);
            }
        }

        protected override void VisitByRef(ByRefTypeReference byRef)
            => byRef.ReferentType.Accept(this);

        protected override void VisitPointer(PointerTypeReference pointer)
            => pointer.ReferentType.Accept(this);

        protected override void VisitConstant(ConstantReference constant)
        {
            constant.DeclaringType.Accept(this);
            _linkBuilder.Append('.').Append(constant.Name);
        }

        protected override void VisitField(FieldReference field)
        {
            field.DeclaringType.Accept(this);
            _linkBuilder.Append('.').Append(field.Name);
        }

        protected override void VisitConstructor(ConstructorReference constructor)
        {
            constructor.DeclaringType.Accept(this);
            _linkBuilder.Append(".-ctor");
        }

        protected override void VisitEvent(EventReference @event)
        {
            @event.DeclaringType.Accept(this);
            _linkBuilder.Append('.').Append(@event.Name);
        }

        protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
        {
        }

        protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
        {
        }

        protected override void VisitProperty(PropertyReference property)
        {
            property.DeclaringType.Accept(this);
            _linkBuilder.Append('.').Append(property.Name);
        }

        protected override void VisitMethod(MethodReference method)
        {
            method.DeclaringType.Accept(this);
            _linkBuilder.Append('.').Append(method.Name);
            if (method.GenericArguments.Any())
                _linkBuilder.Append('-').Append(method.GenericArguments.Count);
        }
    }
}
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars.Visitors
{
    internal sealed class LibrarySelectorVisitor : MemberReferenceVisitor
    {
        public AssemblyReference Library { get; private set; }

        protected override void VisitAssembly(AssemblyReference assembly)
            => Library = assembly;

        protected override void VisitNamespace(NamespaceReference @namespace)
            => Library = @namespace.Assembly;

        protected override void VisitType(TypeReference type)
            => Library = type.Assembly;

        protected override void VisitArray(ArrayTypeReference array)
            => array.ItemType.Accept(this);

        protected override void VisitByRef(ByRefTypeReference byRef)
            => byRef.ReferentType.Accept(this);

        protected override void VisitPointer(PointerTypeReference pointer)
            => pointer.ReferentType.Accept(this);

        protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            => Library = genericTypeParameter.DeclaringType.Assembly;

        protected override void VisitConstant(ConstantReference constant)
            => Library = constant.DeclaringType.Assembly;

        protected override void VisitField(FieldReference field)
            => Library = field.DeclaringType.Assembly;

        protected override void VisitConstructor(ConstructorReference constructor)
            => Library = constructor.DeclaringType.Assembly;

        protected override void VisitEvent(EventReference @event)
            => Library = @event.DeclaringType.Assembly;

        protected override void VisitProperty(PropertyReference property)
            => Library = property.DeclaringType.Assembly;

        protected override void VisitMethod(MethodReference method)
            => Library = method.DeclaringType.Assembly;

        protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
            => Library = genericMethodParameter.DeclaringMethod.DeclaringType.Assembly;
    }
}
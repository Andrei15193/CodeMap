using CodeMap.ReferenceData;

namespace CodeMap.Tests.ReferenceData
{
    public class MemberReferenceVisitorAdapter : MemberReferenceVisitor
    {
        private readonly ITestMemberReferenceVisitor _memberReferenceVisitor;

        public MemberReferenceVisitorAdapter(ITestMemberReferenceVisitor memberReferenceVisitor)
        {
            _memberReferenceVisitor = memberReferenceVisitor;
        }

        protected override void VisitType(TypeReference type)
            => _memberReferenceVisitor.VisitType(type);

        protected override void VisitArray(ArrayTypeReference arrayType)
            => _memberReferenceVisitor.VisitArray(arrayType);

        protected override void VisitPointer(PointerTypeReference pointerType)
            => _memberReferenceVisitor.VisitPointer(pointerType);

        protected override void VisitByRef(ByRefTypeReference byRefType)
            => _memberReferenceVisitor.VisitByRef(byRefType);

        protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            => _memberReferenceVisitor.VisitGenericTypeParameter(genericTypeParameter);

        protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
            => _memberReferenceVisitor.VisitGenericMethodParameter(genericMethodParameter);

        protected override void VisitConstant(ConstantReference constant)
            => _memberReferenceVisitor.VisitConstant(constant);

        protected override void VisitField(FieldReference field)
            => _memberReferenceVisitor.VisitField(field);

        protected override void VisitConstructor(ConstructorReference constructor)
            => _memberReferenceVisitor.VisitConstructor(constructor);

        protected override void VisitEvent(EventReference @event)
            => _memberReferenceVisitor.VisitEvent(@event);

        protected override void VisitProperty(PropertyReference property)
            => _memberReferenceVisitor.VisitProperty(property);

        protected override void VisitMethod(MethodReference method)
            => _memberReferenceVisitor.VisitMethod(method);

        protected override void VisitAssembly(AssemblyReference assembly)
            => _memberReferenceVisitor.VisitAssembly(assembly);
    }
}
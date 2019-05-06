using CodeMap.ReferenceData;

namespace CodeMap.Tests.ReferenceData
{
    public interface ITestMemberReferenceVisitor
    {
        void VisitType(TypeReference type);

        void VisitArray(ArrayTypeReference arrayType);

        void VisitPointer(PointerTypeReference pointerType);

        void VisitByRef(ByRefTypeReference byRefType);

        void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter);

        void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter);

        void VisitConstant(ConstantReference constant);

        void VisitField(FieldReference field);

        void VisitConstructor(ConstructorReference constructor);

        void VisitEvent(EventReference @event);

        void VisitProperty(PropertyReference property);

        void VisitMethod(MethodReference method);

        void VisitAssembly(AssemblyReference assembly);
    }
}
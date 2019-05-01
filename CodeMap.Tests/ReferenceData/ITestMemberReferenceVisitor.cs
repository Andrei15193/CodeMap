using CodeMap.ReferenceData;

namespace CodeMap.Tests.ReferenceData
{
    public interface ITestMemberReferenceVisitor
    {
        void VisitType(TypeReference type);

        void VisitArray(ArrayTypeReference arrayType);

        void VisitPointer(PointerTypeReference pointerType);

        void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter);

        void VisitConstant(ConstantReference constant);
    }
}
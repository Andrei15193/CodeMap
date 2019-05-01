using CodeMap.ReferenceData;

namespace CodeMap.Tests.ReferenceData
{
    public interface ITestMemberReferenceVisitor
    {
        void VisitType(TypeReference type);

        void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter);
    }
}
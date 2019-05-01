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

        protected override void VisitArrayType(ArrayTypeReference arrayType)
            => _memberReferenceVisitor.VisitArray(arrayType);

        protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            => _memberReferenceVisitor.VisitGenericTypeParameter(genericTypeParameter);
    }
}
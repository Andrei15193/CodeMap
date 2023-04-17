using CodeMap.ReferenceData;

namespace CodeMap.Documentation
{
    public static class MemberReferenceVisitorExtensions
    {
        public static string GetSimpleNameReference(this MemberReference memberReference)
        {
            var memberReferenceVisitor = new SimpleNameMemberReferenceVisitor();
            memberReference.Accept(memberReferenceVisitor);
            return memberReferenceVisitor.StringBuilder.ToString();
        }

        public static string GetFullNameReference(this MemberReference memberReference)
        {
            var memberReferenceVisitor = new FullNameMemberReferenceVisitor();
            memberReference.Accept(memberReferenceVisitor);
            return memberReferenceVisitor.StringBuilder.ToString();
        }
    }
}
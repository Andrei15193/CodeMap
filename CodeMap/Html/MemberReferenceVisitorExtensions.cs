using CodeMap.ReferenceData;

namespace CodeMap.Html
{
    /// <summary/>
    public static class MemberReferenceVisitorExtensions
    {
        /// <summary/>
        public static string GetSimpleNameReference(this MemberReference memberReference)
        {
            var memberReferenceVisitor = new SimpleNameMemberReferenceVisitor();
            memberReference.Accept(memberReferenceVisitor);
            return memberReferenceVisitor.StringBuilder.ToString();
        }

        /// <summary/>
        public static string GetFullNameReference(this MemberReference memberReference)
        {
            var memberReferenceVisitor = new FullNameMemberReferenceVisitor();
            memberReference.Accept(memberReferenceVisitor);
            return memberReferenceVisitor.StringBuilder.ToString();
        }
    }
}
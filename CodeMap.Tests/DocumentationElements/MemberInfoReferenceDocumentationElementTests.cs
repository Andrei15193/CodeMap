using System;
using System.Linq;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class MemberInfoReferenceDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingInlineMemberInfoReferenceElementWithNullMemberInfoThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("referredMember", () => DocumentationElement.MemberReference(referredMember: null));

            Assert.Equal(new ArgumentNullException("referredMember").Message, exception.Message);
        }

        [Fact]
        public void InlineMemberInfoReferenceElementCallsVisitorMethod()
        {
            var memberReferenceFactory = new MemberReferenceFactory();
            var memberReference = DocumentationElement.MemberReference(memberReferenceFactory.Create(GetType().GetMembers().First()));
            var visitor = new DocumentationVisitorMock<ReferenceDataDocumentationElement>(memberReference);

            memberReference.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}
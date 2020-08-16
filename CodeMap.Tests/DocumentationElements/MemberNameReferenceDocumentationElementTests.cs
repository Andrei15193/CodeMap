using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class MemberNameReferenceDocumentationElementTests : DocumentationElementTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void CreatingInlineMemberNameReferenceElementWithNullValueThrowsException(string canonicalName)
        {
            var exception = Assert.Throws<ArgumentException>("canonicalName", () => DocumentationElement.MemberReference(canonicalName));

            Assert.Equal(new ArgumentException("Cannot be 'null', empty or white space.", "canonicalName").Message, exception.Message);
        }

        [Fact]
        public void InlineMemberNameReferenceElementCallsVisitorMethod()
        {
            var memberReference = DocumentationElement.MemberReference("cannonicalName");
            var visitor = new DocumentationVisitorMock<MemberNameReferenceDocumentationElement>(memberReference);

            memberReference.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}
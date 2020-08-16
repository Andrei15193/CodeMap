using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class RemarksDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingRemarksElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Remarks(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingRemarksElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.Remarks(new BlockDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void RemarksElementCallsBeginningAndEndingVisitorMethods()
        {
            var remarks = DocumentationElement.Remarks();
            var visitor = new DocumentationVisitorMock<RemarksDocumentationElement>(remarks);

            remarks.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}